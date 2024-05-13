import { Code } from '@/components/code/Code';
import { HintText } from '@/components/text';
import type {
	ActionComponentProps,
	BranchActionOutput,
	BranchActionProvider,
} from '../branch-action-base';
import { useBranchActionTranslation } from '../use-branch-action-translation';

const translationKey = 'new-branch-from-here';

const provider: BranchActionProvider = {
	provide(): BranchActionOutput {
		return {
			actionKey: 'new',
			order: 0,
			translationKey,
			ActionComponent: NewBranchFromHere,
		};
	},
};

function NewBranchFromHere({ branches }: ActionComponentProps) {
	const { t } = useBranchActionTranslation(translationKey);
	return (
		<>
			<HintText>{t('how-to-use')}</HintText>
			<Code>{`git new -u ${branches.map((b) => b.name).join(',')}`}</Code>
		</>
	);
}

export default provider;
