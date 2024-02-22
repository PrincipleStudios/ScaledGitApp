import { useTranslation } from 'react-i18next';

export function OverviewComponent() {
	const { t } = useTranslation(['generic']);
	return <>{t('hello-world')}</>;
}
