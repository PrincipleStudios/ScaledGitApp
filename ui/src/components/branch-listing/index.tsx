import { useCallback } from 'react';
import { useTranslation } from 'react-i18next';
import { useForm } from '@principlestudios/react-jotai-forms';
import { useSuspenseQuery } from '@tanstack/react-query';
import type { TFunction } from 'i18next';
import type { Atom } from 'jotai';
import { useAtomValue } from 'jotai';
import { z } from 'zod';
import type { StandardField } from '@/components/form/FieldProps';
import { TextField } from '@/components/form/text-field';
import type { BranchConfiguration } from '@/generated/api/models';
import { queries } from '@/utils/api/queries';

const branchListingSearchSchema = z.object({
	branchName: z.string(),
});

export type BranchItemProps = { branch: BranchConfiguration };

export type BranchListingProps = {
	children: (branches: BranchConfiguration[]) => JSX.Element;
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

	return useCallback(
		function FullBranchListingPresentation({ children }: BranchListingProps) {
			return (
				<>
					<BranchListingForm
						branchName={form.fields.branchName}
						translation={t}
					/>
					<BranchListing branchNameAtom={form.fields.branchName.value}>
						{children}
					</BranchListing>
				</>
			);
		},
		[form.fields.branchName, t],
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
		<form onSubmit={(e) => e.preventDefault()}>
			<TextField field={branchName} translation={t} />
		</form>
	);
}

function BranchListing({
	branchNameAtom,
	children,
}: BranchListingProps & { branchNameAtom: Atom<string> }) {
	const response = useSuspenseQuery(queries.getUpstreamData).data;
	const branchName = useAtomValue(branchNameAtom);
	if (branchName.length === 0) return null;
	const branches = response.filter((r) => r.name.includes(branchName));
	return children(branches);
}
