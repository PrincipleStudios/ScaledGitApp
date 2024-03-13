import type { RecommendationRule } from './rule-base';

export function perBranch(rule: RecommendationRule): RecommendationRule {
	return {
		analyze(branches) {
			return branches.flatMap((branch) => rule.analyze([branch]));
		},
	};
}
