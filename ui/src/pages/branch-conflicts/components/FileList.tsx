import { useTranslation } from 'react-i18next';
import { useLocation } from 'react-router-dom';
import { HiArrowLeft } from 'react-icons/hi2';
import { twMerge } from 'tailwind-merge';
import { Link } from '@/components/common';
import { Heading, HintText } from '@/components/text';
import { toSearchString } from '@/utils/search-string';
import styles from '../inspect.module.css';
import { BranchNamesList } from './BranchNamesList';
import type { FileSelectorProps } from './FileSelector';

export function FileList({ conflict, selected }: FileSelectorProps) {
	const location = useLocation();
	const { t } = useTranslation('branch-conflicts', { keyPrefix: 'inspect' });
	return (
		<div
			className={twMerge(
				'py-4 pl-4 hidden max-h-full md:block overflow-auto',
				styles.filelist,
			)}
		>
			<Link to={{ ...location, pathname: `/graph` }}>
				<HiArrowLeft className="inline-block" /> {t('full-graph')}
			</Link>
			<HintText className="mt-0 mb-2">{t('list')}</HintText>
			<div className="flex flex-col gap-2">
				<BranchNamesList branches={conflict.branches} />
			</div>
			<Link
				to={{
					pathname: `/branch`,
					search: toSearchString({
						name: conflict.branches.map((b) => b.name),
					}),
				}}
			>
				{t('to-graph')}
			</Link>
			<Heading.Section className="my-4">{t('files-header')}</Heading.Section>
			<ul>
				{conflict.files.map((f) => (
					<li key={f.path} className="my-2">
						<Link
							to={{ ...location, pathname: `./${f.path}` }}
							relative="route"
							className={f.path === selected ? 'font-bold' : 'font-normal'}
						>
							{f.path}
						</Link>
					</li>
				))}
			</ul>
		</div>
	);
}
