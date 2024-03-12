import { elementTemplate } from '../templating';

const SectionBase = elementTemplate('Section', 'section', (T) => (
	<T className="my-4" />
));

/** Sections are for regions within an auto-flow area */
export const Section = {
	SingleColumn: SectionBase.extend('Section.SingleColumn', (T) => (
		<T className="max-w-sm" />
	)),
};
