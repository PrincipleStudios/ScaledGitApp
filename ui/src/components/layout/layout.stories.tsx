import type { Meta, StoryObj } from '@storybook/react';
import { HeaderPresentation } from './header.presentation';
import { LayoutPresentation } from './layout.presentation';

const meta: Meta<typeof LayoutPresentation> = {
	title: 'Components/Layout',
	component: LayoutPresentation,
	parameters: {
		layout: 'fullscreen',
	},
	render: (props) => (
		<LayoutPresentation {...props} header={HeaderPresentation} />
	),
};

export default meta;
type Story = StoryObj<typeof LayoutPresentation>;

export const Primary: Story = {};
