import { elementTemplate } from '../templating';

/** SVGs do not respect sizing from grid or flex. In those cases, add an element
 * around the svg and then use this. */
export const FullSizeSvg = elementTemplate('FullSizeSvg', 'svg', (T) => (
	<T className="w-full h-full" />
));
