import { useTranslation } from 'react-i18next';
import { useLocation } from 'react-router-dom';
import { ErrorBoundary } from '../error-boundary/error-boundary';
import { ErrorScreen } from '../errors';

export function withErrorBoundary(
	Component: React.ComponentType,
): React.ComponentType {
	return () => {
		const location = useLocation();
		const { t } = useTranslation(['generic']);
		return (
			<ErrorBoundary
				errorKey={location.pathname + location.search + location.hash}
				fallback={<ErrorScreen message={t('unhandled-error')} />}
			>
				<Component />
			</ErrorBoundary>
		);
	};
}
