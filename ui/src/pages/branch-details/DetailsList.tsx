import { Section } from '../../components/common';
import { Prose } from '../../components/text';
import type { BranchDetails } from '../../generated/api/models';

export function DetailsList({ branches }: { branches: BranchDetails[] }) {
	return (
		<Section.SingleColumn>
			{branches.map((branch) => (
				<Prose key={branch.name}>{branch.name}</Prose>
			))}
		</Section.SingleColumn>
	);
}
