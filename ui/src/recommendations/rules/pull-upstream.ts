import type { QueryClient } from '@tanstack/react-query';
import type {
	Branch,
	BranchDetails,
	UpstreamBranches,
} from '@/generated/api/models';
import { getLocalData } from '@/logic/getLocalData';
import { getRecursiveUpstream } from '@/logic/recursive-upstream';
import { queries } from '@/utils/api/queries';
import { perBranch } from '../per-branch-rule';
import type { RecommendationOutput } from '../rule-base';

const translationKey = 'pull-upstream';
const conflictTranslationKey = 'pull-upstream.conflict';
const upstreamConflictTranslationKey = 'pull-upstream.conflict-upstream';
const reintegrateTranslationKey = 'pull-upstream.reintegrate';

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
			// First, look for an integration branch directly downstream, whose
			// upstreams have no changes.
			const reintegrationCandidates = await getReintegrationCandidates(
				branch,
				allUpstreamData,
				queryClient,
			);

			if (reintegrationCandidates.length > 0) {
				return reintegrationCandidates.map(({ name }) =>
					suggestReintegration(branch.name, name),
				);
			}

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

async function getReintegrationCandidates(
	branch: BranchDetails,
	allUpstreamData: UpstreamBranches,
	queryClient: QueryClient,
) {
	// If there is an integration branch downstream, and its other upstreams are
	// all merged into this branch's upstreams, then recommend reusing the
	// integration branch.
	const integrationBranches = await Promise.all(
		branch.downstream
			.filter((d) => d.type === 'integration')
			.map(async (integ): Promise<null | Branch> => {
				const otherUpstreams =
					allUpstreamData
						.find((d) => d.name === integ.name)
						?.upstream.filter((u) => u.name !== branch.name) ?? [];
				const otherUpstreamData = await Promise.all(
					otherUpstreams.map((u) =>
						queryClient.fetchQuery(queries.getBranchDetails(u.name)),
					),
				);
				// TODO: this verifies that these branches were merged into
				// _their_ upstreams, not our original branch's upstreams. This
				// works correctly only under circumstances where the
				// conflicting branches were merged into a common upstream.
				if (otherUpstreamData.some((d) => d.nonMergeCommitCount > 0))
					return null;

				return integ;
			}),
	);
	return integrationBranches.filter((t): t is NonNullable<typeof t> => !!t);
}

function suggestReintegration(
	target: string,
	integration: string,
): RecommendationOutput {
	return {
		recommendationKey: `${reintegrateTranslationKey}-${target}-${integration}`,
		priority: pullUpstreamPriority,
		translationKey: reintegrateTranslationKey,
		commands: [`git checkout ${target}`, `git merge origin/${integration}`],
		translationParameters: {
			branch: target,
			integration: integration,
		},
	};
}

function reportConflictWithMainBranch(
	target: string,
	incoming: string,
): RecommendationOutput {
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

function reportConflictInUpstreamBranch(
	target: string,
	name: string,
): RecommendationOutput {
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

function reportStandardPullUpstream(
	target: string,
	recurse: boolean,
): RecommendationOutput {
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
