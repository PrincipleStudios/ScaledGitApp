import type { Meta, StoryObj } from '@storybook/react';
import { H1, H2, H3, H4, H5, H6 } from './headings';

const allHeadings = { H1, H2, H3, H4, H5, H6 };

function HeadingStory({
	theme,
	...props
}: { theme: keyof typeof allHeadings } & JSX.IntrinsicElements['h1']) {
	const Component = allHeadings[theme];
	return <Component {...props} />;
}

const meta = {
	title: 'Components/Text/Headings',
	component: H1,
	tags: ['autodocs'],
	argTypes: {
		theme: { options: Object.keys(allHeadings), control: { type: 'select' } },
	},
	args: {
		theme: 'H1',
		children: 'The quick brown fox jumped over the lazy dog',
	},
	render: HeadingStory,
} satisfies Meta<typeof HeadingStory>;

export default meta;

function themeStory(
	theme: keyof typeof allHeadings,
	args?: React.ComponentProps<typeof H1>,
): StoryObj<typeof meta> {
	return {
		args: {
			theme,
			...args,
		},
	};
}

export const H1Story = themeStory('H1');
export const H2Story = themeStory('H2');
export const H3Story = themeStory('H3');
export const H4Story = themeStory('H4');
export const H5Story = themeStory('H5');
export const H6Story = themeStory('H6');
