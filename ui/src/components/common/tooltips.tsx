import { elementTemplate } from '../templating';

export const TooltipLine = elementTemplate('TooltipLine', 'p', (T) => (
	<T className="text-nowrap" />
));
