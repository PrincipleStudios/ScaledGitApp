import { useSuspensePromise } from '../utils/useSuspensePromise';
import { loadAllRules } from './load-all-rules';
import type { RecommendationsEngine } from './rule-base';

export type { RecommendationsEngine, Recommendation } from './rule-base';

export function useRecommendationsEngine(): RecommendationsEngine {
	const allRules = useSuspensePromise(loadAllRules());

	return {
		getRecommendations(branches) {
			return allRules
				.flatMap((rule) => rule.analyze(branches))
				.sort((a, b) => a.priority - b.priority);
		},
	};
}
