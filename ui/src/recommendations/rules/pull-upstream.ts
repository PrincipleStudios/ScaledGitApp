import type { BranchDetails } from '@/generated/api/models';
import { getLocalData } from '@/logic/getLocalData';
import { getRecursiveUpstream } from '@/logic/recursive-upstream';
import { queries } from '@/utils/api/queries';
import { perBranch } from '../per-branch-rule';

const translationKey = 'pull-upstream';
const conflictTranslationKey = 'pull-upstream.conflict';
const upstreamConflictTranslationKey = 'pull-upstream.conflict-upstream';

// Should be lower than "this is unused", but higher than many other
// recommendations.
const pullUpstreamPriority = 10;

export default perBranch({
	async analyze([branch], { queryClient }) {
		const allUpstreamData = await queryClient.fetchQuery(
			queries.getUpstreamData,
		);

		// Get all upstream branches currently in the query cache and filter to
		// those that are behind their immediate upstreams or have conflicts
		// with their immediate upstreams.
		const relevantBranches = [
			branch.name,
			...getRecursiveUpstream(branch.name, allUpstreamData),
		]
			.map((u) => getLocalData(queryClient, queries.getBranchDetails(u)))
			.flatMap((u) => (u ? [u] : []))
			.filter((b) =>
				b.upstream.some((u) => u.behindCount > 0 || u.hasConflict),
			);

		// If nothing has incoming changes or conflicts, exit early
		if (!relevantBranches.length) return [];
		const conflicts = relevantBranches.filter((b) =>
			b.upstream.some((u) => u.hasConflict),
		);

		// If there are conflicts with only the current branch, report how to resolve those.
		if (isArrayOfCurrentBranch(conflicts)) {
			// TODO: if there is an integration branch downstream, and its other
			// upstreams are all merged into this branch's upstreams, then
			// recommend reusing the integration branch. (This behavior is worth
			// an `await`.)

			return branch.upstream
				.filter((u) => u.hasConflict)
				.map(({ name }) => reportConflictWithMainBranch(branch.name, name));
		}

		// If there are conflicts further up the upstream list, suggest resolving those first.
		if (conflicts.length) {
			// resolve upstream branch conflicts first
			return conflicts
				.filter((b) => b !== branch)
				.map(({ name }) => reportConflictInUpstreamBranch(branch.name, name));
		}

		// Otherwise, this is a standard pull-upstream, which may need to be recursive.
		const recurse = !isArrayOfCurrentBranch(relevantBranches);
		return [reportStandardPullUpstream(branch.name, recurse)];

		function isArrayOfCurrentBranch(sample: BranchDetails[]) {
			return sample.length === 1 && sample[0].name === branch.name;
		}
	},
});

function reportConflictWithMainBranch(target: string, incoming: string) {
	return {
		recommendationKey: `${conflictTranslationKey}-${target}-${incoming}`,
		priority: pullUpstreamPriority,
		translationKey: conflictTranslationKey,
		commands: [`git checkout ${target}`, `git merge origin/${incoming}`],
		translationParameters: {
			branch: target,
			incoming: incoming,
		},
	};
}

function reportConflictInUpstreamBranch(target: string, name: string) {
	return {
		recommendationKey: `${upstreamConflictTranslationKey}-${target}-${name}`,
		priority: pullUpstreamPriority,
		translationKey: upstreamConflictTranslationKey,
		seeAlso: [
			{
				key: 'branch',
				url: `#/branch?name=${encodeURIComponent(name)}`,
				translationKey: `seeAlso`,
				translationParameters: {
					branch: name,
				},
			},
		],
		translationParameters: {
			branch: name,
		},
	};
}

function reportStandardPullUpstream(target: string, recurse: boolean) {
	return {
		recommendationKey: `${translationKey}-${target}`,
		priority: pullUpstreamPriority,
		translationKey,
		commands: [
			`git pull-upstream ${target} ${recurse ? '-recurse' : ''}`.trimEnd(),
		],
		translationParameters: {
			branch: target,
		},
	};
}
