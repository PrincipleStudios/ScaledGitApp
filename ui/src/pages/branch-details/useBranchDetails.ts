import {
	useQueryClient,
	useSuspenseQueries,
	useQueries,
} from '@tanstack/react-query';
import type { BranchDetails } from '@/generated/api/models';
import { queries } from '@/utils/api/queries';

export function useBranchDetails(names: string[]) {
	const queryClient = useQueryClient();

	useSuspenseQueries({
		queries: names.map(queries.getBranchDetails),
	});

	const currentResults = getRelevantBranchNames(names, getDetails);
	const result = useQueries({
		queries: currentResults.map(queries.getBranchDetails),
	});
	return result
		.filter((r): r is typeof r & { isSuccess: true } => r.isSuccess)
		.map((r) => r.data);

	function getDetails(name: string) {
		return queryClient.getQueryData<BranchDetails>(
			queries.getBranchDetails(name).queryKey,
		);
	}
}

const upstreamLimit = 50;
const downstreamLimit = 50;

function getRelevantBranchNames(
	names: string[],
	getDetails: (branch: string) => BranchDetails | undefined,
) {
	return [
		...names,
		...expandBranches(
			names,
			getDownstreamBranchNames(getDetails),
			downstreamLimit,
		),
		...expandBranches(names, getUpstreamBranchNames(getDetails), upstreamLimit),
	];
}

function getUpstreamBranchNames(
	getDetails: (branch: string) => BranchDetails | undefined,
) {
	return (current: string) =>
		getDetails(current)?.upstream.map((b) => b.name) ?? [];
}

function getDownstreamBranchNames(
	getDetails: (branch: string) => BranchDetails | undefined,
) {
	return (current: string) =>
		getDetails(current)?.downstream.map((b) => b.name) ?? [];
}

function expandBranches(
	branchNames: string[],
	getMore: (name: string) => string[],
	limit: number,
) {
	const result = new Set<string>(branchNames);
	const stack = [...branchNames];
	let current: string | undefined;
	while ((current = stack.pop())) {
		const adding = getMore(current);

		for (const entry of adding) {
			if (result.has(entry)) continue;
			result.add(entry);
			stack.push(entry);
			if (result.size > limit) break;
		}
		if (result.size > limit) break;
	}
	return Array.from(result.values());
}
