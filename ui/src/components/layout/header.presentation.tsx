import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';
import { twMerge } from 'tailwind-merge';
import styles from './layout.module.css';

export type HeaderProps = {
	/* This type intentionally blank */
};

export function HeaderPresentation(/* {}: HeaderProps */) {
	const { t } = useTranslation(['app']);
	return (
		<header
			className={twMerge(
				styles.header,
				'bg-slate-200 text-slate-900 dark:bg-slate-800 dark:text-white shadow-sm flex flex-row items-center gap-4 h-12 p-1',
			)}
		>
			<Link to="/">{t('title')}</Link>
		</header>
	);
}
