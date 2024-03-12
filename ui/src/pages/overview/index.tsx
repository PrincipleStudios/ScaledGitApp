import { Container } from '../../components/common';
import { useBranchListing } from './branch-listing';

export function OverviewComponent() {
	const BranchListing = useBranchListing();
	return (
		<Container.Flow>
			<BranchListing />
		</Container.Flow>
	);
}
