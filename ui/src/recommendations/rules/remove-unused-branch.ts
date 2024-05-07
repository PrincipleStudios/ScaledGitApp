import { perBranch } from '../per-branch-rule';

const translationKey = 'remove-unused-branch';

export default perBranch({
	analyze([branch]) {
		if (branch.nonMergeCommitCount > 0) return [];
		if (branch.upstream.length !== 1) return [];
		// integration branches have their own checks to see if they are invalid
		if (branch.type === 'integration') return [];

		const commands = [
			`git refactor-upstream -source ${branch.name} -target ${branch.upstream[0].name} -remove`,
		];
		if (branch.exists) commands.push(`git push origin :${branch.name}`);

		return [
			{
				recommendationKey: `${translationKey}-${branch.name}`,
				priority: 0,
				translationKey,
				commands,
				translationParameters: {
					branch: branch.name,
				},
			},
		];
	},
});
