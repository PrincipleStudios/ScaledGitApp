import { useMemo } from 'react';
import type { RouteObject } from 'react-router-dom';
import { Navigate, useLocation, useRoutes } from 'react-router-dom';
import { useSuspenseQuery } from '@tanstack/react-query';
import {
	withParam,
	withPathParamsValue,
	withSearchParamsValue,
} from '@/components/router';
import { queries } from '@/utils/api/queries';
import { InspectConflictDetails } from './inspect';
import { BranchConflictsSummary } from './summary';

const withSearchParamsName = withSearchParamsValue('name');
const branchConflictsSummary = withSearchParamsName(BranchConflictsSummary);
const withFilePathSplat = withPathParamsValue('filePath', 'splat');
const inspectConflictDetails = withFilePathSplat(InspectConflictDetails);

export function BranchConflictsComponent({ name }: { name: string[] }) {
	const conflictDetails = useSuspenseQuery(
		queries.getConflictDetails(name),
	).data;
	const location = useLocation();

	const route = useRoutes(
		useMemo(
			() => [
				{ path: '/', Component: branchConflictsSummary },
				...conflictDetails.flatMap((conflict, i): RouteObject[] => [
					{
						path: `/inspect/${i}/*`,
						Component: withParam('conflict', conflict)(inspectConflictDetails),
					},
				]),
				{
					path: '*',
					element: (
						<Navigate to={{ ...location, pathname: '.' }} relative="route" />
					),
				},
			],
			[conflictDetails, location],
		),
	);

	return route;
}
