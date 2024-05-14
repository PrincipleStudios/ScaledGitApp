import { useMemo } from 'react';
import type { RouteObject } from 'react-router-dom';
import { Navigate, useLocation, useRoutes } from 'react-router-dom';
import { useSuspenseQuery } from '@tanstack/react-query';
import { withParam, withSearchParamsValue } from '@/components/router';
import { queries } from '@/utils/api/queries';
import { InspectConflictDetails } from './inspect';
import { BranchConflictsSummary } from './summary';

const withSearchParamsName = withSearchParamsValue('name');
const branchConflictsSummary = withSearchParamsName(BranchConflictsSummary);

export function BranchConflictsComponent({ name }: { name: string[] }) {
	const conflictDetails = useSuspenseQuery(
		queries.getConflictDetails(name),
	).data;
	const location = useLocation();

	const route = useRoutes(
		useMemo(
			() => [
				{ path: '/', Component: branchConflictsSummary },
				...conflictDetails.map(
					(conflict, i): RouteObject => ({
						path: `/inspect/${i}`,
						Component: withParam('conflict', conflict)(InspectConflictDetails),
					}),
				),
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
