import { Tab as HeadlessuiTab } from '@headlessui/react';
import { twMerge } from 'tailwind-merge';
import { elementTemplate } from '../templating';

export const Tab = Object.assign(
	elementTemplate('Tab', HeadlessuiTab, (T) => <T />, {
		useProps: ({
			className,
		}: {
			className?: string;
		}): React.ComponentProps<typeof HeadlessuiTab> => ({
			className: ({ selected }: { selected: boolean }) =>
				twMerge(
					'w-full rounded-lg py-2.5 text-sm font-medium leading-5',
					'ring-white/60 ring-offset-2 ring-offset-blue-400 focus:outline-none focus:ring-2',
					selected
						? 'bg-white text-blue-700 shadow'
						: 'text-gray-900 hover:bg-white/[0.12] hover:text-gray-700',
					className,
				),
		}),
	}),
	{
		List: elementTemplate('Tab.List', HeadlessuiTab.List, (T) => (
			<T className="flex space-x-1 rounded-xl bg-blue-900/20 p-1" />
		)),
		Panel: elementTemplate('TabPanel', HeadlessuiTab.Panel, (T) => (
			<T
				className={twMerge(
					'rounded-xl bg-white p-3',
					'ring-white/60 ring-offset-2 ring-offset-blue-400 focus:outline-none focus:ring-2',
				)}
			/>
		)),
		Group: HeadlessuiTab.Group,
		Panels: HeadlessuiTab.Panels,
	},
);
