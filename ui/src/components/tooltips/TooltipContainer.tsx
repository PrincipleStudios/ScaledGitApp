import { twMerge } from 'tailwind-merge';

export function TooltipContainer({ children }: { children?: React.ReactNode }) {
	return (
		<div
			className={twMerge(
				'bg-white text-black dark:bg-black dark:text-white border-blue-800',
				'dark:bg-black dark:text-white dark:border-blue-200',
				'p-[1px] border rounded-sm text-xs',
				'absolute bottom-0',
			)}
		>
			{children}
		</div>
	);
}
