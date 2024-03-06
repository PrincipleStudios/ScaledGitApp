import type { UseFieldResult } from '@principlestudios/react-jotai-forms';
import type { DefaultFormFieldResultFlags } from '@principlestudios/react-jotai-forms/internals/useFormHelpers';

export type StandardField<TTarget> = UseFieldResult<
	TTarget,
	DefaultFormFieldResultFlags
>;

export type FieldProps<TTarget> = {
	field: StandardField<TTarget>;
};
