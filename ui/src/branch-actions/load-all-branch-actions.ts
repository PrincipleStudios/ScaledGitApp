import memoize from 'lodash/memoize';
import type { BranchActionProvider } from './branch-action-base';

const rules = import.meta.glob('./actions/*.tsx');
async function baseLoadAllBranchActions() {
	const allRulesModules = await Promise.all(
		Object.values(rules).map((load) => load()),
	);
	return allRulesModules.map(
		(module) => (module as { default: BranchActionProvider }).default,
	);
}
export const loadAllBranchActions = memoize(baseLoadAllBranchActions);
