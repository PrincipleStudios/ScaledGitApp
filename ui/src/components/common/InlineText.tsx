import { elementTemplate } from '../templating';

export const InlineText = Object.assign(
	elementTemplate('InlineText', 'span', (T) => (
		<T className="inline-flex flex-row align-baseline gap-x-1" />
	)),
	{
		Icon: elementTemplate('InlineText.Icon', 'span', (T) => (
			<T className="inline-block w-4 h-4 self-center" />
		)),
	},
);
