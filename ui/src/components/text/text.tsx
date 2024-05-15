/* eslint-disable jsx-a11y/heading-has-content */
import { elementTemplate } from '../templating';

export const Prose = elementTemplate('Prose', 'p', (T) => (
	<T className="leading-loose text-opacity-90 my-4" />
));

export const HintText = elementTemplate('HintText', 'p', (T) => (
	<T className="leading-tight text-sm italic text-opacity-100 my-4" />
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
