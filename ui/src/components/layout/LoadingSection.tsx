import { Suspense } from 'react';
import { useTranslation } from 'react-i18next';

export function LoadingSection({ children }: { children?: React.ReactNode }) {
	const { t } = useTranslation(['generic']);
	return <Suspense fallback={t('loading')}>{children}</Suspense>;
}
