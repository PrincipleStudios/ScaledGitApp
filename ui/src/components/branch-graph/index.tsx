import { useCallback } from 'react';
import { useSuspenseQuery } from '@tanstack/react-query';
import { queries } from '../../utils/api/queries';
import { BranchGraphPresentation } from './branch-graph.presentation';

export function useBranchGraph() {
	const response = useSuspenseQuery(queries.getUpstreamData);
	return useCallback(
		() => <BranchGraphPresentation upstreamData={response.data} />,
		[response.data],
	);
}
