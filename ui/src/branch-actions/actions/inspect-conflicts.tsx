import { useSuspenseQuery } from '@tanstack/react-query';
import { Link } from '@/components/common';
import { Prose } from '@/components/text';
import { queries } from '@/utils/api/queries';
import { toSearchString } from '@/utils/search-string';
import type {
	ActionComponentProps,
	BranchActionProvider,
} from '../branch-action-base';
import { useBranchActionTranslation } from '../use-branch-action-translation';

const translationKey = 'inspect-conflicts';

const provider: BranchActionProvider = {
	async provide({ branches, queryClient }) {
		try {
			const conflictDetails = await queryClient.fetchQuery(
				queries.getConflictDetails(branches.map((b) => b.name)),
			);

			// No conflicts!
			if (conflictDetails.conflicts.length === 0) return null;

			return {
				actionKey: translationKey,
				order: -10,
				translationKey,
				ActionComponent: InspectConflicts,
			};
		} catch (_ex) {
			return null;
		}
	},
};

function InspectConflicts({ branches }: ActionComponentProps) {
	const { t } = useBranchActionTranslation(translationKey);
	const conflictDetails = useSuspenseQuery(
		queries.getConflictDetails(branches.map((b) => b.name)),
	).data;
	const branchNames = branches.map((b) => b.name);

	return (
		<>
			<Prose>{t('conflict-count', { count: conflictDetails.length })}</Prose>
			<Link
				to={{
					pathname: '/branch/conflicts',
					search: toSearchString({ name: branchNames }),
				}}
			>
				{t('see-conflicts')}
			</Link>
		</>
	);
}

export default provider;
