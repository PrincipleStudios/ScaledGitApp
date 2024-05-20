import { useRef } from 'react';
import { Editor } from '@monaco-editor/react';
import { useSuspenseQuery } from '@tanstack/react-query';
import { type editor } from 'monaco-editor';
import { twMerge } from 'tailwind-merge';
import type {
	ConflictDetails,
	FileConflictDetails,
} from '@/generated/api/models';
import { queries } from '@/utils/api/queries';

const fileExtension = /\.(?<extension>[^./]+)$/;
const fileTypes: Readonly<Record<string, string>> = {
	css: 'css',
	js: 'javascript',
	json: 'json',
	md: 'markdown',
	mjs: 'javascript',
	ts: 'typescript',
};

function useFile(tree: string, path: string) {
	const fileData = useSuspenseQuery(
		queries.retrieveGitObject(`${tree}:${path}`),
	).data;

	return fileData;
}

type EditorInstance = editor.IStandaloneCodeEditor;

export function ShowFileConflicts({
	conflict,
	file,
	className,
}: {
	conflict: ConflictDetails;
	file: FileConflictDetails;
	className?: string;
}) {
	const editorRef = useRef<EditorInstance>();
	const diff = useFile(conflict.mergeTree, file.path);
	const languageMatch = fileExtension.exec(file.path);
	const language = fileTypes[languageMatch?.groups?.['extension'] ?? ''];

	// TODO - handle missing, deleted, different modes, etc.

	if (editorRef.current) {
		// The Editor is not controlled; we must manually update it on rerenders.
		editorRef.current.setValue(diff.data);
	}

	return (
		<div className={twMerge('z-0', className)}>
			<Editor
				value={diff.data}
				language={language}
				onMount={(editor) => (editorRef.current = editor)}
				options={{
					automaticLayout: true,
					wordWrap: 'on',
					readOnly: true,
				}}
			/>
		</div>
	);
}
