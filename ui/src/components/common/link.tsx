import { Link as RouterLink } from 'react-router-dom';
import { twMerge } from 'tailwind-merge';
import { elementTemplate } from '../templating';

const linkClasses = 'text-blue-800 dark:text-blue-200 underline font-bold';

export const Link = elementTemplate('Link', RouterLink, (T) => (
	<T className={linkClasses} />
));

export const ExternalLink = elementTemplate('ExternalLink', 'a', (T) => (
	<T className={linkClasses} />
));

export const LinkButton = elementTemplate('Link', 'button', (T) => (
	<T type="button" className={twMerge(linkClasses, 'contents')} />
));
