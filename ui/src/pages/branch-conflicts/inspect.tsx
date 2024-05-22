import { Container, Section } from '@/components/common';
import type { ConflictDetails } from '@/generated/api/models';
import { ConflictSummary } from './summary';

export function InspectConflictDetails({
	conflict,
}: {
	conflict: ConflictDetails;
}) {
	// TODO - don't use the summary, but instead use a monaco editor
	return (
		<Container.Flow>
			<Section.SingleColumn>
				<ConflictSummary conflict={conflict} />
			</Section.SingleColumn>
		</Container.Flow>
	);
}
