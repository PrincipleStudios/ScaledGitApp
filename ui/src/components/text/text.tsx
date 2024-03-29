import { elementTemplate } from '../templating';

export const Prose = elementTemplate('Prose', 'p', (T) => (
	<T className="leading-loose text-opacity-90 my-4" />
));

export const HintText = elementTemplate('HintText', 'p', (T) => (
	<T className="leading-tight text-sm italic text-opacity-100 my-4" />
));

export const Code = elementTemplate('Code', 'pre', (T) => (
	<T className="font-mono bg-zinc-300 dark:bg-zinc-700 text-black dark:text-white p-4 border-zinc-500 rounded my-4 overflow-x-auto" />
));

const HeadingBase = elementTemplate('Heading', 'h2', (T) => (
	<T className="text-xl font-bold" />
));
export const Heading = HeadingBase.themed({
	Section: () => <h3 className="text-lg" />,
});

export const LargeInstructions = elementTemplate(
	'LargeInstructions',
	'p',
	(T) => (
		<T className="text-2xl text-slate-900 dark:text-slate-100 font-serif" />
	),
);
