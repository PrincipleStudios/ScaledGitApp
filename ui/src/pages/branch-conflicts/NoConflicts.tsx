import { useTranslation } from 'react-i18next';
import { Container, Link, Section } from '@/components/common';
import { Prose } from '@/components/text';
import { toSearchString } from '@/utils/search-string';

export function NoConflicts({ name }: { name: string[] }) {
	const { t } = useTranslation('branch-conflicts', {
		keyPrefix: 'no-conflicts',
	});
	return (
		<Container.Flow>
			<Section.SingleColumn>
				<Prose>{t('no-conflicts')}</Prose>
				<Link
					to={{
						pathname: `/branch`,
						search: toSearchString({ name }),
					}}
				>
					{t('to-graph')}
				</Link>
			</Section.SingleColumn>
		</Container.Flow>
	);
}
