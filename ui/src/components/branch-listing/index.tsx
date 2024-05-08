import { useCallback } from 'react';
import { useTranslation } from 'react-i18next';
import { useForm } from '@principlestudios/react-jotai-forms';
import { useSuspenseQuery } from '@tanstack/react-query';
import type { TFunction } from 'i18next';
import { useAtomValue } from 'jotai';
import { z } from 'zod';
import { BulletList, Section } from '@/components/common';
import type { StandardField } from '@/components/form/FieldProps';
import { TextField } from '@/components/form/text-field';
import type { BranchConfiguration } from '@/generated/api/models';
import { queries } from '@/utils/api/queries';

const branchListingSearchSchema = z.object({
	branchName: z.string(),
});

export type BranchItemProps = { branch: BranchConfiguration };

export type BranchListingProps = {
	branchItem: React.ComponentType<BranchItemProps>;
};

export function useBranchListing() {
	const { t } = useTranslation('branch-listing', { keyPrefix: 'form' });
	const form = useForm({
		defaultValue: { branchName: '' },
		schema: branchListingSearchSchema,
		fields: {
			branchName: ['branchName'],
		},
		preSubmit: 'all',
	});
	const response = useSuspenseQuery(queries.getUpstreamData).data;

	const BranchListing = useCallback(
		function BranchListing(props: BranchListingProps) {
			const branchName = useAtomValue(form.fields.branchName.value);
			if (branchName.length === 0) return null;
			const branches = response.filter((r) => r.name.includes(branchName));
			return <BranchListingPresentation branches={branches} {...props} />;
		},
		[response, form.fields.branchName.value],
	);

	return useCallback(
		function FullBranchListingPresentation(props: BranchListingProps) {
			return (
				<>
					<BranchListingForm
						branchName={form.fields.branchName}
						translation={t}
					/>
					<BranchListing {...props} />
				</>
			);
		},
		[form.fields.branchName, BranchListing, t],
	);
}

function BranchListingForm({
	branchName,
	translation: t,
}: {
	branchName: StandardField<string>;
	translation: TFunction;
}) {
	return (
		<Section.SingleColumn>
			<form onSubmit={(e) => e.preventDefault()}>
				<TextField field={branchName} translation={t} />
			</form>
		</Section.SingleColumn>
	);
}

export function BranchListingPresentation({
	branches,
	branchItem: BranchItem,
}: {
	branches: BranchConfiguration[];
} & BranchListingProps) {
	return (
		<Section.SingleColumn>
			<BulletList>
				{branches.map((b) => (
					<BulletList.Item key={b.name}>
						<BranchItem branch={b} />
					</BulletList.Item>
				))}
			</BulletList>
		</Section.SingleColumn>
	);
}
