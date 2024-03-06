import { useCallback } from 'react';
import { useTranslation } from 'react-i18next';
import { useForm } from '@principlestudios/react-jotai-forms';
import { useSuspenseQuery } from '@tanstack/react-query';
import { useAtomValue } from 'jotai';
import { z } from 'zod';
import { BulletList, Link, Section } from '../../components/common';
import { TextField } from '../../components/form/text-field';
import { queries } from '../../utils/api/queries';
import type { StandardField } from '../../components/form/FieldProps';

const branchListingSearchSchema = z.object({
	branchName: z.string(),
});

export function useBranchListing() {
	const { t } = useTranslation('branch-listing', { keyPrefix: 'form' });
	const form = useForm({
		defaultValue: { branchName: '' },
		schema: branchListingSearchSchema,
		translation: t,
		fields: {
			branchName: ['branchName'],
		},
		preSubmit: 'all',
	});
	const response = useSuspenseQuery(queries.getUpstreamData).data;

	const BranchListing = useCallback(
		function BranchListing() {
			const branchName = useAtomValue(form.fields.branchName.value);
			if (branchName.length === 0) return null;
			const branches = response
				.filter((r) => r.name.includes(branchName))
				.map((r) => r.name)
				.filter((r, i) => i < 100);
			return <BranchListingPresentation branches={branches} />;
		},
		[response, form.fields.branchName.value],
	);

	return useCallback(
		function FullBranchListingPresentation() {
			return (
				<>
					<BranchListingForm branchName={form.fields.branchName} />
					<BranchListing />
				</>
			);
		},
		[form.fields.branchName, BranchListing],
	);
}

function BranchListingForm({
	branchName,
}: {
	branchName: StandardField<string>;
}) {
	return (
		<Section.SingleColumn>
			<form onSubmit={(e) => e.preventDefault()}>
				<TextField field={branchName} />
			</form>
		</Section.SingleColumn>
	);
}

export function BranchListingPresentation({
	branches,
}: {
	branches: string[];
}) {
	return (
		<Section.SingleColumn>
			<BulletList>
				{branches.map((b) => (
					<BulletList.Item key={b}>
						<Link to={`/branch?name=${b}`}>{b}</Link>
					</BulletList.Item>
				))}
			</BulletList>
		</Section.SingleColumn>
	);
}
