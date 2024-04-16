import type { Meta, StoryObj } from '@storybook/react';
import { Prose, HintText, LargeInstructions } from './text';

const meta = {
	title: 'Components/Text/Blocks',
	component: Prose,
	parameters: {
		layout: 'centered',
	},
	tags: ['autodocs'],
	argTypes: {},
	args: {
		children: 'The quick brown fox jumped over the lazy dog',
	},
} satisfies Meta<React.ComponentType<React.HTMLAttributes<HTMLElement>>>;

export default meta;

function themeStory(
	Target: React.ComponentType<React.HTMLAttributes<HTMLElement>>,
	args?: React.ComponentProps<typeof Prose>,
): StoryObj<typeof meta> {
	return {
		render: (args) => <Target {...args} />,
		args: {
			...args,
		},
	};
}

export const ProseStory = themeStory(Prose);
export const HintTextStory = themeStory(HintText);
export const LargeInstructionsStory = themeStory(LargeInstructions);
