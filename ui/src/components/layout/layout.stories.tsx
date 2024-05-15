import type { Meta, StoryObj } from '@storybook/react';
import { HeaderPresentation } from './header.presentation';
import type { HeaderPresentationalProps } from './header.presentation';
import { LayoutPresentation } from './layout.presentation';

function StorybookHeaderPresentation(props: HeaderPresentationalProps) {
	return (
		<HeaderPresentation
			{...props}
			isRefreshing={false}
			onRefresh={() => {}}
			onSearch={() => {}}
		/>
	);
}

const meta: Meta<typeof LayoutPresentation> = {
	title: 'Components/Layout',
	component: LayoutPresentation,
	parameters: {
		layout: 'fullscreen',
	},
	render: (props) => (
		<LayoutPresentation {...props} header={StorybookHeaderPresentation} />
	),
};

export default meta;
type Story = StoryObj<typeof LayoutPresentation>;

export const Primary: Story = {};
