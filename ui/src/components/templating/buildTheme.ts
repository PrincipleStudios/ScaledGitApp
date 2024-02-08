import type { ThemedTemplateResolver } from './types';

/**
 * Captures types of a theme without needing to explicitly type each property.
 * @param theme The strongly-typed theme
 * @returns The theme passed
 */
// eslint-disable-next-line @typescript-eslint/no-explicit-any
export function buildTheme<TKeys extends string, TProps = any>(
	theme: ThemedTemplateResolver<TKeys, TProps>,
) {
	return theme;
}
