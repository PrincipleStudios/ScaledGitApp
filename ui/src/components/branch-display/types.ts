import type {
	Branch,
	BranchConfiguration,
	BranchDetails,
} from '../../generated/api/models';

export type BranchInfo =
	| (Branch & {
			detailed: false;
	  })
	| (BranchConfiguration & { detailed: true });

export function isDetailed(branch: Branch): branch is BranchDetails {
	return branch ? 'nonMergeCommitCount' in branch : false;
}
