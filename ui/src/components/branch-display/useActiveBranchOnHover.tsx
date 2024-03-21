import { useEffect } from 'react';
import { useSetAtom } from 'jotai';
import { activeBranchNames } from './active';
import type { BranchInfo } from './types';

export function useActiveBranchOnHover(branch: BranchInfo) {
	const setActiveBranches = useSetAtom(activeBranchNames);
	useEffect(function onUnmountRemoveFromActiveBranches() {
		return () => setActiveBranches({ remove: branch.name });
	});

	return {
		onMouseEnter: () => setActiveBranches({ add: branch.name }),
		onMouseLeave: () => setActiveBranches({ remove: branch.name }),
	};
}
