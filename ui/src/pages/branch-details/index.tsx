import { useNavigate } from 'react-router-dom';
import {
	useQueryClient,
	useSuspenseQueries,
	useQueries,
} from '@tanstack/react-query';
import { BranchGraphPresentation } from '../../components/branch-graph/branch-graph.presentation';
import { Container } from '../../components/common';
import { queries } from '../../utils/api/queries';
import { getBranchDetails } from '../../utils/api/queries/git/branch-details';
import type { BranchDetails } from '../../generated/api/models';

export function BranchDetailsComponent({ name }: { name: string[] }) {
	const navigate = useNavigate();
	const response = useBranchDetails(name);
	return (
		<Container.Full>
			<BranchGraphPresentation
				upstreamData={response}
				onClick={(node) => navigate({ search: `?name=${node.name}` })}
			/>
		</Container.Full>
	);
}

function useBranchDetails(names: string[]) {
	const queryClient = useQueryClient();

	useSuspenseQueries({
		queries: names.map(getBranchDetails),
	});

	const currentResults = getRelevantBranchNames(names, getDetails);
	const result = useQueries({
		queries: currentResults.map(getBranchDetails),
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

function getRelevantBranchNames(
	names: string[],
	getDetails: (branch: string) => BranchDetails | undefined,
) {
	return [
		...names,
		...expandBranches(names, getUpstreamBranchNames(getDetails)),
		...expandBranches(names, getDownstreamBranchNames(getDetails)),
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
		}
	}
	return Array.from(result.values());
}
