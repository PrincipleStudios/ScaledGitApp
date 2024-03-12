import { Link as RouterLink } from 'react-router-dom';
import { elementTemplate } from '../templating';

export const Link = elementTemplate('Link', RouterLink, (T) => (
	<T className="text-blue-800 dark:text-blue-200 underline font-bold" />
));
