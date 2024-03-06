import { useSuspenseQuery } from '@tanstack/react-query';
import { BranchGraphPresentation } from '../../components/branch-graph/branch-graph.presentation';
import { Container } from '../../components/common';
import { queries } from '../../utils/api/queries';

export function BranchDetailsComponent({ name }: { name: string[] }) {
	const response = useSuspenseQuery(queries.getBranchDetails(name)).data;
	return (
		<Container.Full>
			<BranchGraphPresentation upstreamData={response} />
		</Container.Full>
	);
}
