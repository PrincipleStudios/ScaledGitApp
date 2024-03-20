import { currentValue } from '@principlestudios/jotai-utilities/currentValue';
import { atom } from 'jotai';
import type { RecommendationOutput, RecommendationRule } from './rule-base';

export function perBranch(rule: RecommendationRule): RecommendationRule {
	return {
		analyze(branches, context) {
			return atom(async (get) => {
				const result = (
					await Promise.all(
						branches.map((branch) => {
							const ruleResult = rule.analyze([branch], context);
							return currentValue<
								RecommendationOutput[] | Promise<RecommendationOutput[]>
							>(ruleResult, get);
						}),
					)
				).flatMap((e) => e);
				return result;
			});
		},
	};
}
