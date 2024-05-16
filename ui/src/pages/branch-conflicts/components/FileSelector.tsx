import { useTranslation } from 'react-i18next';
import { useLocation, useNavigate } from 'react-router-dom';
import { useForm } from '@principlestudios/react-jotai-forms';
import { useAtomValue } from 'jotai';
import { twMerge } from 'tailwind-merge';
import { z } from 'zod';
import { SelectField } from '@/components/form/select-field';
import type { ConflictDetails } from '@/generated/api/models';
import styles from '../inspect.module.css';

export type FileSelectorProps = {
	conflict: ConflictDetails;
	selected: string | undefined;
};

const filePathSelectorSchema = z.object({
	filePath: z.string(),
});
export function FileSelector({ conflict, selected }: FileSelectorProps) {
	const navigate = useNavigate();
	const location = useLocation();
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

	const actual = useAtomValue(form.fields.filePath.atom);
	if (actual !== selected) {
		navigate({ ...location, pathname: `./${actual}` });
	}

	return (
		<div className={twMerge('p-4 md:hidden', styles.fileselector)}>
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
