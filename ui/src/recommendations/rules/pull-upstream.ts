import { perBranch } from '../per-branch-rule';

const translationKey = 'pull-upstream';
const conflictTranslationKey = 'pull-upstream.conflict';

// Should be lower than "this is unused", but higher than many other
// recommendations.
const pullUpstreamPriority = 10;

export default perBranch({
	analyze([branch]) {
		if (!branch.upstream.some((u) => u.behindCount > 0)) return [];

		if (branch.upstream.some((u) => u.hasConflict)) {
			return branch.upstream
				.filter((u) => u.hasConflict)
				.map(({ name }) => ({
					recommendationKey: `${conflictTranslationKey}-${branch.name}-${name}`,
					priority: pullUpstreamPriority,
					translationKey: conflictTranslationKey,
					commands: [`git checkout ${branch.name}`, `git merge origin/${name}`],
					translationParameters: {
						branch: branch.name,
						incoming: name,
					},
				}));
		}

		const commands = [`git pull-upstream ${branch.name}`];
		return [
			{
				recommendationKey: `${translationKey}-${branch.name}`,
				priority: pullUpstreamPriority,
				translationKey,
				commands,
				translationParameters: {
					branch: branch.name,
				},
			},
		];
	},
});
