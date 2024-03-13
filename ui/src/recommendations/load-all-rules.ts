import memoize from 'lodash/memoize';
import type { RecommendationRule } from './rule-base';

const rules = import.meta.glob('./rules/*.ts');
async function baseLoadAllRules() {
	const allRulesModules = await Promise.all(
		Object.values(rules).map((load) => load()),
	);
	return allRulesModules.map(
		(module) => (module as { default: RecommendationRule }).default,
	);
}
export const loadAllRules = memoize(baseLoadAllRules);
