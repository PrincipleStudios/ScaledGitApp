import { buildTheme } from '../templating';

export const buttonThemes = buildTheme({
	Secondary: (T) => (
		<T className="bg-slate-100 text-slate-900 focus:bg-slate-50 hover:bg-slate-50 dark:bg-slate-900 dark:text-slate-100 dark:focus:bg-slate-950 dark:hover:bg-slate-950" />
	),
});
