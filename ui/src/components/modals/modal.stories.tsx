import type { Meta, StoryObj } from '@storybook/react';
import { Button } from '@/components/common';
import { Prose } from '@/components/text';
import { Modal } from '@/utils/modal/modal';
import { ModalDialogLayout } from './modal-dialog';

function SampleModal({
	children,
	...props
}: React.ComponentProps<typeof Modal>) {
	return (
		<div className="h-64">
			<Modal {...props}>{children}</Modal>
		</div>
	);
}

const meta = {
	title: 'Components/Modal',
	component: Modal,
	render: SampleModal,
	parameters: {
		layout: 'fullscreen',
	},
	argTypes: {},
	args: {},
} satisfies Meta<typeof Modal>;
type Story = StoryObj<typeof meta>;

export default meta;

export const BasicModal: Story = {
	args: {
		label: 'Details',
		children: (
			<ModalDialogLayout
				title="Details"
				buttons={
					<>
						<Button>OK</Button>
						<Button.Secondary>Cancel</Button.Secondary>
					</>
				}
			>
				<Prose>
					This is the details of the document. Is it full HTML? A form? Hard to
					be sure, but this story will give us an idea.
				</Prose>
			</ModalDialogLayout>
		),
	},
};
