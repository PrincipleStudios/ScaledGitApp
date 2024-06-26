import { useTranslation } from 'react-i18next';
import { useForm } from '@principlestudios/react-jotai-forms';
import { useAtomValue } from 'jotai';
import { z } from 'zod';
import { BranchName } from '@/components/branch-display/BranchName';
import { Section } from '@/components/common';
import { Details } from '@/components/details';
import { SelectField } from '@/components/form/select-field';
import { translateField } from '@/components/form/utils/translations';
import { Prose } from '@/components/text';
import type {
	BranchDetails,
	DetailedUpstreamBranch,
} from '@/generated/api/models';
import { findBranch, namesOf } from './utils';

const detailsSchema = z.object({
	mainBranch: z.string(),
	upstreamBranch: z.string().nullable(),
});

export function DetailsPanel({ branches }: { branches: BranchDetails[] }) {
	const { t } = useTranslation('branch-details');
	const form = useForm({
		defaultValue: {
			mainBranch: branches[0].name,
			upstreamBranch: null,
		},
		schema: detailsSchema,
		fields: {
			mainBranch: ['mainBranch'],
			upstreamBranch: ['upstreamBranch'],
		},
	});

	const { mainBranch: mainBranchName, upstreamBranch: upstreamBranchName } =
		useAtomValue(form.atom);
	const mainBranch = findBranch(branches, mainBranchName);
	const upstreamBranch =
		mainBranch && findBranch(mainBranch.upstream, upstreamBranchName);

	const upstreamBranchTranslation = translateField(
		form.fields.upstreamBranch,
		t,
	);

	return (
		<Section.SingleColumn>
			<SelectField
				field={form.fields.mainBranch}
				translation={t}
				items={namesOf(branches)}
			>
				{(branchName) => (
					<BranchName data={branches.find((b) => b.name === branchName)!} />
				)}
			</SelectField>
			{mainBranch ? <BranchStatePresentation branch={mainBranch} /> : null}
			{mainBranch?.upstream.length ? (
				<SelectField
					field={form.fields.upstreamBranch}
					translation={t}
					items={namesOf(mainBranch.upstream)}
				>
					{(branchName) => {
						// Ensures the branch named actually exists in the main branch
						const branch = findBranch(mainBranch.upstream, branchName);
						return branch ? (
							<BranchName data={branch} />
						) : (
							upstreamBranchTranslation(['none-selected'])
						);
					}}
				</SelectField>
			) : (
				<Prose>{t('no-upstream-branches')}</Prose>
			)}
			{upstreamBranch ? (
				<UpstreamBranchStatePresentation branch={upstreamBranch} />
			) : null}
		</Section.SingleColumn>
	);
}

function BranchStatePresentation({ branch }: { branch: BranchDetails }) {
	const { t } = useTranslation('branch-details', { keyPrefix: 'main-details' });
	return (
		<Section.SingleColumn>
			<Details>
				<Details.Entry label={t('name')}>
					<BranchName data={branch} />
				</Details.Entry>
				<Details.Entry label={t('exists')}>
					{branch.exists ? t('exists-true') : t('exists-false')}
				</Details.Entry>
				<Details.Entry label={t('non-merge-commits')}>
					{branch.nonMergeCommitCount}
				</Details.Entry>
			</Details>
		</Section.SingleColumn>
	);
}

function UpstreamBranchStatePresentation({
	branch,
}: {
	branch: DetailedUpstreamBranch;
}) {
	const { t } = useTranslation('branch-details', {
		keyPrefix: 'upstream-details',
	});
	return (
		<Section.SingleColumn>
			<Details>
				<Details.Entry label={t('name')}>
					<BranchName data={branch} />
				</Details.Entry>
				<Details.Entry label={t('exists')}>
					{branch.exists ? t('exists-true') : t('exists-false')}
				</Details.Entry>
				<Details.Entry label={t('behind-count')}>
					{branch.behindCount}
				</Details.Entry>
				<Details.Entry label={t('has-conflict')}>
					{branch.hasConflict
						? t('has-conflict-true')
						: t('has-conflict-false')}
				</Details.Entry>
			</Details>
		</Section.SingleColumn>
	);
}
