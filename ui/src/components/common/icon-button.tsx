import { Button } from './button';

export const IconButton = Button.extend('Button', (T) => (
	<T className={'p-2 rounded-full text-xl w-auto'} />
));
