import { useForm } from '@principlestudios/react-jotai-forms';
import { useAtomValue } from 'jotai';
import { z } from 'zod';
import { Code } from '@/components/code/Code';
import { TextField } from '@/components/form/text-field';
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

const newBranchNameSchema = z.object({
	branchName: z.string().regex(/^[a-zA-Z0-9_/-]*$/),
});

function NewBranchFromHere({ branches }: ActionComponentProps) {
	const { t } = useBranchActionTranslation(translationKey);
	const form = useForm({
		defaultValue: { branchName: '' },
		schema: newBranchNameSchema,
		fields: {
			branchName: ['branchName'],
		},
		preSubmit: 'all',
	});

	const newBranchName = useAtomValue(form.fields.branchName.atom);
	const branchList = branches.map((b) => b.name).join(',');

	return (
		<>
			<HintText>{t('how-to-use')}</HintText>
			<form onSubmit={(e) => e.preventDefault()}>
				<TextField field={form.fields.branchName} translation={t} />
			</form>
			{newBranchName ? null : <HintText>{t('prompt')}</HintText>}
			<Code>{`git new${newBranchName ? ' ' + newBranchName : ''} -upstreamBranches ${branchList}`}</Code>
		</>
	);
}

export default provider;
