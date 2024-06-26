import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';
import { HiArrowPath, HiMagnifyingGlass } from 'react-icons/hi2';
import { IconButton } from '../common';
import { elementTemplate } from '../templating';
import styles from './layout.module.css';

export type HeaderPresentationalProps = {
	/* This type intentionally blank */
};

export type HeaderProps = HeaderPresentationalProps & {
	isRefreshing: boolean;
	onSearch: () => void;
	onRefresh: () => void;
};

const Header = elementTemplate('Header', 'header', (T) => (
	<T className="bg-slate-200 text-slate-900 dark:bg-slate-800 dark:text-white shadow-sm flex flex-row items-center gap-4 h-12 py-1 px-4" />
));

export function HeaderPresentation({
	isRefreshing,
	onSearch,
	onRefresh,
}: HeaderProps) {
	const { t } = useTranslation(['app']);
	return (
		<Header className={styles.header}>
			<Link to="/">{t('title')}</Link>
			<span className="flex-grow" />
			<IconButton onClick={onSearch}>
				<HiMagnifyingGlass title={t('search')} />
			</IconButton>
			<IconButton
				title={t('refresh')}
				onClick={onRefresh}
				disabled={isRefreshing}
			>
				<HiArrowPath
					title={t('refresh')}
					className={isRefreshing ? 'animate-spin' : ''}
				/>
			</IconButton>
		</Header>
	);
}
