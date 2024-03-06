import { elementTemplate } from '../templating';

const SectionBase = elementTemplate('Section', 'section', (T) => (
	<T className="m-4" />
));

export const Section = {
	SingleColumn: SectionBase.extend('Section.SingleColumn', (T) => (
		<T className="max-w-sm" />
	)),
};
