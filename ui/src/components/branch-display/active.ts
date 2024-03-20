import { atomWithReducer } from 'jotai/utils';

function branchListReducer(
	prev: string[],
	action: { add: string } | { remove: string },
) {
	if ('add' in action) {
		if (prev.includes(action.add)) return prev;
		return [...prev, action.add];
	}

	const index = prev.indexOf(action.remove);
	if (index === -1) return prev;
	return prev.filter((branchName, i) => i !== index);
}

// Site-wide branch
export const activeBranchNames = atomWithReducer([], branchListReducer);
