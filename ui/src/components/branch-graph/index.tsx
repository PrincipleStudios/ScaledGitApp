import { useCallback } from 'react';
import { queries } from '@/utils/api/queries';
import { useSuspenseQuery } from '@tanstack/react-query';
import { BranchGraphPresentation } from './branch-graph.presentation';

export function useBranchGraph() {
	const response = useSuspenseQuery(queries.getUpstreamData);
	return useCallback(
		() => <BranchGraphPresentation upstreamData={response.data} />,
		[response.data],
	);
}
