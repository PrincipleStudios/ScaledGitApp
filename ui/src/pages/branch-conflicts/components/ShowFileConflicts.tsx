import { DiffEditor } from '@monaco-editor/react';
import type { UseSuspenseQueryOptions } from '@tanstack/react-query';
import { useSuspenseQuery } from '@tanstack/react-query';
import { twMerge } from 'tailwind-merge';
import type { FileConflictDetails, FileSnapshot } from '@/generated/api/models';
import { queries } from '@/utils/api/queries';

const allowedFileModes: ReadonlyArray<string | undefined> = [
	'100644',
	'100755',
];
const unknownBlob = {
	queryKey: ['unknown-blob'],
	queryFn: () => Promise.resolve(null),
} satisfies UseSuspenseQueryOptions;

const fileExtension = /\.(?<extension>[^./]+)$/;
const fileTypes: Readonly<Record<string, string>> = {
	css: 'css',
	js: 'javascript',
	json: 'json',
	md: 'markdown',
	mjs: 'javascript',
	ts: 'typescript',
};

function useFileSnapshot(file: FileSnapshot | undefined) {
	const fileData = useSuspenseQuery(
		allowedFileModes.includes(file?.mode)
			? queries.retrieveGitObject(file?.hash ?? '')
			: unknownBlob,
	).data;

	return {
		mode: file?.mode,
		...fileData,
	};
}

export function ShowFileConflicts({
	file,
	className,
}: {
	file: FileConflictDetails;
	className?: string;
}) {
	const left = useFileSnapshot(file.left);
	const right = useFileSnapshot(file.right);
	// const mergeBase = useFileSnapshot(file.mergeBase);
	const languageMatch = fileExtension.exec(file.path);
	const language = fileTypes[languageMatch?.groups?.['extension'] ?? ''];

	// TODO - handle missing, deleted, different modes, etc.

	return (
		<div className={twMerge('z-0', className)}>
			<DiffEditor
				original={left.data}
				modified={right.data}
				originalLanguage={language}
				modifiedLanguage={language}
			/>
		</div>
	);
}
