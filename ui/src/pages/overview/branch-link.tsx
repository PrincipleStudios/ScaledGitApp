import type { BranchItemProps } from '@/components/branch-listing';
import { Link } from '@/components/common';

export function BranchLink({ branch }: BranchItemProps) {
	return <Link to={`/branch?name=${branch.name}`}>{branch.name}</Link>;
}
