import { getRecursiveUpstream } from '@/logic/recursive-upstream';
import { queries } from '@/utils/api/queries';
import { perBranch } from '../per-branch-rule';

const translationKey = 'remove-invalid-integration';
// Invalid checks for integration branches should be ranked low; conflicts are
// difficult work and should not be trashed lightly.
const invalidIntegrationOrder = 100;

export default perBranch({
	async analyze([branch], { queryClient }) {
		if (branch.type !== 'integration') return [];

		const allUpstreamData = await queryClient.fetchQuery(
			queries.getUpstreamData,
		);

		return branch.upstream
			.filter((upstream) => {
				// if `upstream` has all the other upstreams recursively upstream...
				// or if there is no upstream that is not in the recursive list
				const recursive = getRecursiveUpstream(upstream.name, allUpstreamData);
				return !branch.upstream.some(
					(u) => u.name !== upstream.name && !recursive.includes(u.name),
				);
			})
			.map((target) => {
				const commands = [
					`git refactor-upstream -source ${branch.name} -target ${target.name} -remove`,
				];
				if (branch.exists) commands.push(`git push origin :${branch.name}`);
				return {
					recommendationKey: `${translationKey}-${branch.name}`,
					priority: invalidIntegrationOrder,
					translationKey,
					commands,
					translationParameters: {
						branch: branch.name,
					},
				};
			});
	},
});
