import { atomWithReducer } from 'jotai/utils';

function branchListReducer(
	prev: string[],
	action: { add: string } | { remove: string },
) {
	if ('add' in action) return [...prev, action.add];

	const index = prev.indexOf(action.remove);
	if (index !== -1) return prev.filter((branchName, i) => i !== index);
	return prev;
}

// Site-wide branch
export const activeBranchNames = atomWithReducer([], branchListReducer);
