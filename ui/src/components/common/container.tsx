import { elementTemplate } from '../templating';

const ContainerBase = elementTemplate('Container', 'div', (T) => (
	<T className="p-4" />
));

/** Containers are full-width regions with horizontal and vertical padding. Do
 * not use a non-flow container with a layout in conjunction with sections. */
export const Container = {
	Flow: elementTemplate('Container.Flow', 'div', (T) => (
		<T className="px-4 h-full overflow-auto" />
	)),
	Full: ContainerBase.extend('Container.Full', (T) => (
		<T className="w-full h-full grid grid-cols-1 grid-rows-1" />
	)),
	Responsive: elementTemplate('Container.GridBase', 'div', (T) => (
		<T className="px-4 w-full min-h-full overflow-auto md:overflow-hidden md:py-4 md:grid gap-4" />
	)),
};
