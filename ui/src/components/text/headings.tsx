/* eslint-disable jsx-a11y/heading-has-content */
import { elementTemplate } from '../templating';

const heading = elementTemplate('heading', 'h1', (T) => (
	<T className="font-bold leading-relaxed mt-6 mb-4 border-b border-zinc-300 dark:boder-zinc-700" />
));
export const H1 = heading.extend('H1', () => <h1 className="text-6xl" />);
export const H2 = heading.extend('H2', () => <h2 className="text-5xl" />);
export const H3 = heading.extend('H3', () => <h3 className="text-4xl" />);
export const H4 = heading.extend('H4', () => <h4 className="text-3xl" />);
export const H5 = heading.extend('H5', () => <h5 className="text-2xl" />);
export const H6 = heading.extend('H6', () => <h6 className="text-xl" />);
