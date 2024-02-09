declare module 'tailwindcss/lib/util/flattenColorPalette' {
	import type { RecursiveKeyValuePair } from 'tailwindcss/types/config';
	import type { DefaultColors } from 'tailwindcss/types/generated/colors';

	export default function flattenColorPalette(
		param: DefaultColors | RecursiveKeyValuePair<string, string> | undefined,
	): Record<string, string>;
}
