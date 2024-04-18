import type { RecommendationRule } from './rule-base';

export function perBranch(rule: RecommendationRule): RecommendationRule {
	return {
		analyze(branches, context) {
			const analysis = branches
				.map((branch) => rule.analyze([branch], context))
				.flatMap((e) => (Array.isArray(e) ? e : [e]));
			return analysis;
		},
	};
}
