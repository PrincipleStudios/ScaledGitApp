import { useBranchListing } from '@/components/branch-listing';
import { Container } from '@/components/common';
import { BranchLink } from './branch-link';

export function OverviewComponent() {
	const BranchListing = useBranchListing();
	return (
		<Container.Flow>
			<BranchListing branchItem={BranchLink} />
		</Container.Flow>
	);
}
