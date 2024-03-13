import { useTranslation } from 'react-i18next';
import { useForm } from '@principlestudios/react-jotai-forms';
import { z } from 'zod';
import { Section } from '../../components/common';
import { SelectField } from '../../components/form/select-field';
import { Prose } from '../../components/text';
import type { BranchDetails } from '../../generated/api/models';

const detailsSchema = z.object({
	mainBranch: z.string(),
	upstreamBranch: z.string().nullable(),
});

export function DetailsList({ branches }: { branches: BranchDetails[] }) {
	const { t } = useTranslation('branch-details');
	const form = useForm({
		defaultValue: {
			mainBranch: branches[0].name,
			upstreamBranch: null,
		},
		schema: detailsSchema,
		translation: t,
		fields: {
			mainBranch: {
				path: ['mainBranch'],
				readOnly: () => branches.length === 1,
			},
			upstreamBranch: ['upstreamBranch'],
		},
	});
	return (
		<Section.SingleColumn>
			<SelectField
				field={form.fields.mainBranch}
				items={branches.map((b) => b.name)}
			>
				{(branchName) => branchName}
			</SelectField>
			{branches.map((branch) => (
				<Prose key={branch.name}>{branch.name}</Prose>
			))}
		</Section.SingleColumn>
	);
}
