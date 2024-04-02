import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';
import { Button } from '../common';
import { elementTemplate } from '../templating';
import styles from './layout.module.css';

export type HeaderPresentationalProps = {
	/* This type intentionally blank */
};

export type HeaderProps = HeaderPresentationalProps & {
	isRefreshing: boolean;
	onRefresh: () => void;
};

const Header = elementTemplate('Header', 'header', (T) => (
	<T className="bg-slate-200 text-slate-900 dark:bg-slate-800 dark:text-white shadow-sm flex flex-row items-center gap-4 h-12 p-1" />
));

export function HeaderPresentation({ isRefreshing, onRefresh }: HeaderProps) {
	const { t } = useTranslation(['app']);
	return (
		<Header className={styles.header}>
			<Link to="/">{t('title')}</Link>
			<span className="flex-grow" />
			<Button onClick={onRefresh} disabled={isRefreshing}>
				{t('refresh')}
			</Button>
		</Header>
	);
}
