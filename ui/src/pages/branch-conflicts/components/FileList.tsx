import { useTranslation } from 'react-i18next';
import { useLocation } from 'react-router-dom';
import { twMerge } from 'tailwind-merge';
import { Link } from '@/components/common';
import { Heading, HintText } from '@/components/text';
import styles from '../inspect.module.css';
import { BranchNamesList } from './BranchNamesList';
import type { FileSelectorProps } from './FileSelector';

export function FileList({ conflict, selected }: FileSelectorProps) {
	const location = useLocation();
	const { t } = useTranslation('branch-conflicts', { keyPrefix: 'inspect' });
	return (
		<div className={twMerge('py-4 pl-4 hidden md:block', styles.filelist)}>
			<HintText className="mt-0 mb-2">{t('list')}</HintText>
			<div className="flex flex-col gap-2">
				<BranchNamesList branches={conflict.branches} />
			</div>
			<Heading.Section className="my-4">{t('files-header')}</Heading.Section>
			<ul>
				{conflict.files.map((f) => (
					<li key={f.path} className="my-2">
						<Link
							to={{ ...location, pathname: `./${f.path}` }}
							relative="route"
							className={f.path === selected ? 'font-bold' : undefined}
						>
							{f.path}
						</Link>
					</li>
				))}
			</ul>
		</div>
	);
}
