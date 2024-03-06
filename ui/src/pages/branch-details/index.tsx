import { useSuspenseQuery } from '@tanstack/react-query';
import { BranchGraphPresentation } from '../../components/branch-graph/branch-graph.presentation';
import { queries } from '../../utils/api/queries';

export function BranchDetailsComponent({ name }: { name: string }) {
	const response = useSuspenseQuery(queries.getBranchDetails(name)).data;
	return <BranchGraphPresentation upstreamData={response} />;
}
