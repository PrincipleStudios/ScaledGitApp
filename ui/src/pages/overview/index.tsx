import { useBranchListing } from '@/components/branch-listing';
import { BulletList, Container, Section } from '@/components/common';
import { BranchLink } from './branch-link';

export function OverviewComponent() {
	const BranchListing = useBranchListing();
	return (
		<Container.Flow>
			<Section.SingleColumn>
				<BranchListing>
					{(branches) => (
						<Section.SingleColumn>
							<BulletList>
								{branches.map((b) => (
									<BulletList.Item key={b.name}>
										<BranchLink branch={b} />
									</BulletList.Item>
								))}
							</BulletList>
						</Section.SingleColumn>
					)}
				</BranchListing>
			</Section.SingleColumn>
		</Container.Flow>
	);
}
