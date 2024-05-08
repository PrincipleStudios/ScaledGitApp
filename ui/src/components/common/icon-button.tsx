import { Button } from './button';
import { buttonThemes } from './button-themes';

export const IconButton = Button.extend('Button', (T) => (
	<T className={'p-2 rounded-full text-xl w-auto'} />
)).themed(buttonThemes);
