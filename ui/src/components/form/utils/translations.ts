import type { AnyPath } from '@principlestudios/react-jotai-forms';
import type { TFunction } from 'i18next';

export type FieldTranslation = (additionalParts: AnyPath) => string;
export function translateField(
	{ translationPath }: { translationPath?: AnyPath },
	t: TFunction,
): FieldTranslation {
	return (additionalParts) =>
		t(['fields', ...(translationPath ?? []), ...additionalParts].join('.'));
}
