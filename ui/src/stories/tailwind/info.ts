/* eslint-disable @typescript-eslint/no-unsafe-argument */
import tailwindConfig from '../../../tailwind.config';
import resolveConfig from 'tailwindcss/resolveConfig';
import flattenColorPalette from 'tailwindcss/lib/util/flattenColorPalette';

const fullConfig = resolveConfig(tailwindConfig);

if (!fullConfig.theme) throw new Error('tailwind config failed to parse');

export const colors = flattenColorPalette(fullConfig.theme.colors);
delete colors.transparent;
delete colors.current;
delete colors.inherit;

export const fontFamilies = fullConfig.theme.fontFamily ?? {};
const fontSizesFromTheme = fullConfig.theme.fontSize ?? {};
export const fontSizes = Object.fromEntries(
	Object.entries(fontSizesFromTheme).map(([sizeName, config]) => [
		sizeName,
		toFontSizeStyle(config),
	]),
);
type FontSizeOptions = {
	lineHeight?: string;
	letterSpacing?: string;
	fontWeight?: string | number;
};
function toFontSizeStyle(
	value:
		| string
		| [fontSize: string]
		| [fontSize: string, lineHeight: string]
		| [fontSize: string, options: FontSizeOptions],
): React.CSSProperties {
	const [fontSize, options] = Array.isArray(value) ? value : [value];

	const { lineHeight, letterSpacing, fontWeight } =
		typeof options === 'object'
			? options
			: ({ lineHeight: options } as FontSizeOptions);

	return {
		fontSize,
		lineHeight,
		letterSpacing,
		fontWeight,
	};
}
