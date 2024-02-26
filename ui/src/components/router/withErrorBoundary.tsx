import { useTranslation } from 'react-i18next';
import { useResolvedPath } from 'react-router-dom';
import { ErrorBoundary } from '../error-boundary/error-boundary';
import { ErrorScreen } from '../errors';

export function withErrorBoundary(
	Component: React.ComponentType,
): React.ComponentType {
	return () => {
		const { pathname } = useResolvedPath('./');
		const { t } = useTranslation(['generic']);
		return (
			<ErrorBoundary
				errorKey={pathname}
				fallback={<ErrorScreen message={t('unhandled-error')} />}
			>
				<Component />
			</ErrorBoundary>
		);
	};
}
