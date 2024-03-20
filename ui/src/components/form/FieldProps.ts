import type {
	AnyPath,
	DefaultFormFieldResultFlags,
	UseFieldResult,
} from '@principlestudios/react-jotai-forms';
import type { TFunction } from 'i18next';

export type StandardField<TTarget> = UseFieldResult<
	TTarget,
	DefaultFormFieldResultFlags
> & {
	translationPath?: AnyPath;
};

export type FieldProps<TTarget> = {
	field: StandardField<TTarget>;
	translation: TFunction;
};
