import { Fragment } from 'react';
import { useSuspenseQueries } from '@tanstack/react-query';
import { BranchName } from '@/components/branch-display/BranchName';
import type { Branch } from '@/generated/api/models';
import { queries } from '@/utils/api/queries';

export function BranchNamesList({ branches }: { branches: Branch[] }) {
	const fullBranchDetails = useSuspenseQueries({
		queries: branches.map((b) => queries.getBranchDetails(b.name)),
	}).map((d) => d.data);

	return (
		<>
			{fullBranchDetails.map((b) => (
				<Fragment key={b.name}>
					<BranchName data={b} />{' '}
				</Fragment>
			))}
		</>
	);
}
