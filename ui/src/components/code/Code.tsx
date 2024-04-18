import { useMutation } from '@tanstack/react-query';
import { HiOutlineClipboardDocument, HiCheck } from 'react-icons/hi2';
import { elementTemplate } from '../templating';

const CodeContainer = elementTemplate('CodeContainer', 'div', (T) => (
	<T className="relative rounded my-4 clear-both overflow-hidden" />
));
const CopyButton = elementTemplate('CopyButton', 'button', (T) => (
	<T
		type="button"
		className="float-right m-2 p-2 border rounded-sm border-black bg-zinc-200 dark:bg-zinc-800"
	/>
));
const CodeDisplay = elementTemplate('CodeDisplay', 'pre', (T) => (
	<T className="font-mono bg-zinc-300 dark:bg-zinc-700 text-black dark:text-white p-4 border-zinc-500 text-wrap" />
));

export function Code({
	children,
	...props
}: { children: string } & React.HTMLAttributes<HTMLDivElement>) {
	const { isSuccess, reset, mutate } = useMutation({
		mutationFn: async () => {
			await navigator.clipboard.writeText(children);
		},
		onMutate: cleanup,
	});
	return (
		<CodePresentation copied={isSuccess} onCopyCode={mutate} {...props}>
			{children}
		</CodePresentation>
	);

	function cleanup() {
		setTimeout(reset, 1000);
	}
}

function CodePresentation({
	children,
	copied,
	onCopyCode,
	...props
}: {
	children: string;
	copied: boolean;
	onCopyCode: () => void;
} & React.HTMLAttributes<HTMLDivElement>) {
	return (
		<CodeContainer {...props}>
			<CopyButton
				title="Copy"
				className={copied ? 'text-green-700 dark:text-green-400' : undefined}
				onClick={onCopyCode}
			>
				{copied ? <HiCheck /> : <HiOutlineClipboardDocument />}
			</CopyButton>
			<CodeDisplay>{children}</CodeDisplay>
		</CodeContainer>
	);
}
