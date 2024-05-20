import { twMerge } from 'tailwind-merge';
import { Container } from '@/components/common';
import type { ConflictDetails } from '@/generated/api/models';
import { FileList } from './components/FileList';
import { FileSelector } from './components/FileSelector';
import { ShowFileConflicts } from './components/ShowFileConflicts';
import { UnknownFile } from './components/UnknownFile';
import styles from './inspect.module.css';

export function InspectConflictDetails({
	conflict,
	filePath,
}: {
	conflict: ConflictDetails;
	filePath?: string | undefined;
}) {
	const selectedFile =
		conflict.files.find((f) => f.path === filePath) ?? conflict.files[0];

	// TODO - don't use the summary, but instead use a monaco editor
	return (
		<Container.Responsive
			className={twMerge('px-0 md:py-0 grid', styles.container)}
		>
			<FileList conflict={conflict} selected={selectedFile?.path} />
			<FileSelector conflict={conflict} selected={selectedFile?.path} />

			{selectedFile ? (
				<ShowFileConflicts file={selectedFile} className={styles.details} />
			) : (
				<UnknownFile className={styles.details} />
			)}
		</Container.Responsive>
	);
}
