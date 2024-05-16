import { Fragment } from 'react';
import { BranchName } from '@/components/branch-display/BranchName';
import type { Branch } from '@/generated/api/models';

export function BranchNamesList({ branches }: { branches: Branch[] }) {
	return (
		<>
			{branches.map((b) => (
				<Fragment key={b.name}>
					<BranchName data={b} />{' '}
				</Fragment>
			))}
		</>
	);
}
