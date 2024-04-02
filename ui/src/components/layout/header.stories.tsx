import type { Meta, StoryObj } from '@storybook/react';
import { HeaderPresentation } from './header.presentation';

const meta: Meta<typeof HeaderPresentation> = {
	title: 'Components/Layout/Header',
	component: HeaderPresentation,
	args: {
		isRefreshing: false,
	},
	parameters: {
		layout: 'fullscreen',
	},
	render: (props) => <HeaderPresentation {...props} />,
};

export default meta;
type Story = StoryObj<typeof HeaderPresentation>;

export const Primary: Story = {};
