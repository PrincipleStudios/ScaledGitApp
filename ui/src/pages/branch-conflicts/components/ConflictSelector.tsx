import { useTranslation } from 'react-i18next';
import { useLocation, useNavigate } from 'react-router-dom';
import { useForm } from '@principlestudios/react-jotai-forms';
import type { PrimitiveAtom } from 'jotai';
import { useStore } from 'jotai';
import { twMerge } from 'tailwind-merge';
import { z } from 'zod';
import { SelectField } from '@/components/form/select-field';
import { translateField } from '@/components/form/utils/translations';
import { HintText } from '@/components/text';
import type { ConflictAnalysis } from '@/generated/api/models';
import { useAtomEffect } from '@/utils/atoms/useAtomEffect';
import styles from '../inspect.module.css';
import { BranchNamesList } from './BranchNamesList';

export type ConflictSelectorProps = {
	conflicts: ConflictAnalysis;
	selected: string | undefined;
};

const filePathSelectorSchema = z.object({
	conflictIndex: z.number().int(),
});
export function ConflictSelector({
	conflicts,
	selected,
}: ConflictSelectorProps) {
	const { t } = useTranslation('branch-conflicts', { keyPrefix: 'inspect' });
	const selectedIndex = isNaN(Number(selected)) ? 0 : Number(selected);
	const form = useForm({
		schema: filePathSelectorSchema,
		defaultValue: {
			conflictIndex: selectedIndex,
		},
		fields: {
			conflictIndex: ['conflictIndex'],
		},
	});

	useLocationAtom(form.fields.conflictIndex.atom, selectedIndex);

	return (
		<div className={styles.conflictselector}>
			{conflicts.conflicts.length === 1 ? (
				<HintText className="mt-0 mb-2">
					{translateField(form.fields.conflictIndex, t)(['label'])}
				</HintText>
			) : (
				<SelectField
					items={conflicts.conflicts.map((conflict, index) => index)}
					translation={t}
					field={form.fields.conflictIndex}
				>
					{(index) =>
						t('conflict-index', {
							replace: { index: (index + 1).toFixed(0) },
						})
					}
				</SelectField>
			)}

			<div className="flex flex-col gap-2">
				<BranchNamesList branches={conflicts.conflicts[0].branches} />
			</div>
		</div>
	);
}

// Keeps the location path synced in this atom
function useLocationAtom(atom: PrimitiveAtom<number>, selected: number) {
	const navigate = useNavigate();
	const location = useLocation();
	// Ensure form and URL stay in sync
	useAtomEffect(atom, (path) => {
		if (selected === path) return;
		navigate({
			...location,
			pathname: `/branch/conflicts/inspect/${path}`,
		});
	});
	const store = useStore();
	const actual = store.get(atom);
	if (actual !== selected && selected) {
		setTimeout(() => store.set(atom, selected), 0);
	}
}
