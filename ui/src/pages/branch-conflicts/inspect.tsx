import { useTranslation } from 'react-i18next';
import { Navigate, useLocation } from 'react-router-dom';
import { HiArrowLeft } from 'react-icons/hi2';
import { twMerge } from 'tailwind-merge';
import { Container } from '@/components/common';
import { Link } from '@/components/common';
import type { ConflictAnalysis } from '@/generated/api/models';
import { ConflictSelector } from './components/ConflictSelector';
import { FileList } from './components/FileList';
import { FileSelector } from './components/FileSelector';
import { ShowFileConflicts } from './components/ShowFileConflicts';
import { UnknownFile } from './components/UnknownFile';
import styles from './inspect.module.css';

export function InspectConflictDetails({
	conflicts,
	index = '0',
	filePath,
}: {
	conflicts: ConflictAnalysis;
	index?: string;
	filePath?: string | undefined;
}) {
	const location = useLocation();
	const { t } = useTranslation('branch-conflicts', { keyPrefix: 'inspect' });

	const conflict = conflicts.conflicts[Number(index)];
	const selectedFile =
		conflict?.files.find((f) => f.path === filePath) ?? conflict?.files[0];

	if (!selectedFile)
		return <Navigate to={{ ...location, pathname: './summary' }} />;

	// TODO - don't use the summary, but instead use a monaco editor
	return (
		<Container.Responsive
			className={twMerge('px-0 md:py-0 grid h-full', styles.container)}
		>
			<div className={twMerge('md:py-4 md:pl-4 overflow-auto', styles.sidebar)}>
				<Link to={{ ...location, pathname: `/branch` }}>
					<HiArrowLeft className="inline-block" /> {t('full-graph')}
				</Link>
				<ConflictSelector conflicts={conflicts} selected={index} />
				<FileList conflict={conflict} selected={selectedFile?.path} />
				<FileSelector conflict={conflict} selected={selectedFile?.path} />
			</div>

			{selectedFile ? (
				<ShowFileConflicts
					conflict={conflict}
					file={selectedFile}
					className={styles.details}
				/>
			) : (
				<UnknownFile className={styles.details} />
			)}
		</Container.Responsive>
	);
}
