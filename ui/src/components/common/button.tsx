import { twMerge } from 'tailwind-merge';
import { elementTemplate } from '../templating';
import { buttonThemes } from './button-themes';

/** Not intended for use as a component (though it could be), this is intended
 * more as a template for a base `button` that moves the "disabled"
 * functionality to a more appropriate location and applies a base style. */
const disabledButtonTemplate = elementTemplate(
	'disabledButton',
	'button',
	(T) => <T type="button" />,
	{
		useProps: ({ className, disabled, ...rest }) => ({
			disabled: false,
			className: twMerge(disabled && 'opacity-20', className),
			...rest,
		}),
	},
);

export const Button = disabledButtonTemplate
	.extend('Button', (T) => (
		<T
			className={twMerge(
				'bg-slate-800 text-white focus:bg-slate-700 hover:bg-slate-700 outline-black',
				'dark:bg-slate-100 dark:text-slate-900 dark:focus:bg-slate-200 dark:hover:bg-slate-200 dark:outline-white',
				'px-3 py-2 rounded-md',
				'w-full sm:w-auto',
				'inline-flex items-center justify-center',
				'text-sm font-semibold',
				'transition-colors shadow-sm',
			)}
		/>
	))
	.themed(buttonThemes);
