import { useTranslation } from 'react-i18next';
import { useLocation, useNavigate } from 'react-router-dom';
import { useForm } from '@principlestudios/react-jotai-forms';
import type { PrimitiveAtom } from 'jotai';
import { useStore } from 'jotai';
import { z } from 'zod';
import { SelectField } from '@/components/form/select-field';
import type { ConflictDetails } from '@/generated/api/models';
import { useAtomEffect } from '@/utils/atoms/useAtomEffect';
import styles from '../inspect.module.css';

export type FileSelectorProps = {
	conflict: ConflictDetails;
	selected: string | undefined;
};

const filePathSelectorSchema = z.object({
	filePath: z.string(),
});
export function FileSelector({ conflict, selected }: FileSelectorProps) {
	const { t } = useTranslation('branch-conflicts', { keyPrefix: 'inspect' });
	const form = useForm({
		schema: filePathSelectorSchema,
		defaultValue: {
			filePath: selected ?? '',
		},
		fields: {
			filePath: ['filePath'],
		},
	});

	useLocationAtom(form.fields.filePath.atom, selected);

	return (
		<div className={styles.fileselector}>
			<SelectField
				items={conflict.files.map((f) => f.path)}
				translation={t}
				field={form.fields.filePath}
			>
				{(p) => p}
			</SelectField>
		</div>
	);
}

// Keeps the location path synced in this atom
function useLocationAtom(
	atom: PrimitiveAtom<string>,
	selected: string | undefined,
) {
	const navigate = useNavigate();
	const location = useLocation();
	// Ensure form and URL stay in sync
	useAtomEffect(atom, (path) => {
		if (selected === path) return;
		navigate(
			{
				...location,
				pathname: `./${path}`,
			},
			{ relative: 'route' },
		);
	});
	const store = useStore();
	const actual = store.get(atom);
	if (actual !== selected && selected) {
		setTimeout(() => store.set(atom, selected), 0);
	}
}
