import { useTranslation } from 'react-i18next';
import { ErrorScreen } from '@/components/errors';

export function UnknownFile({ className }: { className?: string }) {
	const { t } = useTranslation('branch-conflicts');
	return (
		<div className={className}>
			<ErrorScreen message={t('unknown-file')} />
		</div>
	);
}
