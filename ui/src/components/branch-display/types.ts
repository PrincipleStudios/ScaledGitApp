import type {
	Branch,
	BranchConfiguration,
	BranchDetails,
} from '../../generated/api/models';

export type BranchInfo = Branch &
	(Partial<BranchConfiguration> | Partial<BranchDetails>);

export function isDetailed(branch: Branch): branch is BranchDetails {
	return branch ? 'nonMergeCommitCount' in branch : false;
}
