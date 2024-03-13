import type { Meta, StoryObj } from '@storybook/react';
import { Details } from '.';

const meta: Meta = {
	title: 'Components/Details',
	parameters: {
		layout: 'centered',
	},
	argTypes: {},
	render: () => (
		<Details>
			<Details.Entry label="Name">main</Details.Entry>
			<Details.Entry label="Exists?">Present on remote</Details.Entry>
			<Details.Entry label="Commits ahead of upstreams">178</Details.Entry>
		</Details>
	),
};

export default meta;

export const Primary: StoryObj = {};
