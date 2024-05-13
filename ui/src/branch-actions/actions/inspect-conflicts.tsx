import { useSuspenseQuery } from '@tanstack/react-query';
import { queries } from '@/utils/api/queries';
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
			if (conflictDetails.length === 0) return null;

			return {
				actionKey: translationKey,
				order: -10,
				translationKey,
				ActionComponent: InspectConflicts,
			};
		} catch (ex) {
			return null;
		}
	},
};

function InspectConflicts({ branches }: ActionComponentProps) {
	const { t } = useBranchActionTranslation(translationKey);
	const conflictDetails = useSuspenseQuery(
		queries.getConflictDetails(branches.map((b) => b.name)),
	).data;

	return (
		<>
			TODO
			{t('see-conflicts')}
			{JSON.stringify(conflictDetails)}
		</>
	);
}

export default provider;
