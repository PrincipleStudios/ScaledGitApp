import type {
	Branch,
	BranchConfiguration,
	BranchDetails,
} from '@/generated/api/models';

export type BranchInfo = Branch | BranchConfiguration | BranchDetails;

export function isDetailed(branch: Branch): branch is BranchDetails {
	return 'nonMergeCommitCount' in branch;
}

export function isBasic(
	branch: Branch,
): branch is BranchConfiguration | BranchDetails {
	return 'upstream' in branch;
}
