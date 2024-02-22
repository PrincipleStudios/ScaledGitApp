import type { Meta, StoryObj } from '@storybook/react';
import { LayoutPresentation } from './layout.presentation';

const meta: Meta<typeof LayoutPresentation> = {
	title: 'Components/Layout',
	component: LayoutPresentation,
	render: (props) => <LayoutPresentation {...props} />,
};

export default meta;
type Story = StoryObj<typeof LayoutPresentation>;

export const Primary: Story = {};
