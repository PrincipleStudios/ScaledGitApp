import { useMemo } from 'react';
import type { RouteObject } from 'react-router-dom';
import { useRoutes } from 'react-router-dom';
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
const withFilePathSplat = withPathParamsValue('filePath', '*');
const withIndex = withPathParamsValue('index');
const inspectConflictDetails = withIndex(
	withFilePathSplat(InspectConflictDetails),
);

export function BranchConflictsComponent({ name }: { name: string[] }) {
	const conflictDetails = useSuspenseQuery(
		queries.getConflictDetails(name),
	).data;

	const route = useRoutes(
		useMemo(
			(): RouteObject[] => [
				{
					path: `/inspect/:index/*`,
					Component: withParam(
						'conflicts',
						conflictDetails,
					)(inspectConflictDetails),
				},
				{ path: '/summary', Component: branchConflictsSummary },
				{
					path: '*',
					element: <InspectConflictDetails conflicts={conflictDetails} />,
				},
			],
			[conflictDetails],
		),
	);

	return route;
}
