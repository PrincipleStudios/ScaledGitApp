import type { FormEventHandler, ReactNode } from 'react';
import type { TFunction } from 'i18next';
import { Button } from '@/components/common';
import { ModalDialogLayout } from './modal-dialog';

export function ModalForm({
	children,
	title,
	translation: t,
	additionalButtons,
	onSubmit,
	onCancel,
}: {
	children?: ReactNode;
	title?: ReactNode;
	onSubmit?: FormEventHandler<HTMLFormElement>;
	onCancel?: () => void;
	translation: TFunction;
	additionalButtons?: ReactNode;
}) {
	return (
		<form onSubmit={onSubmit}>
			<ModalDialogLayout
				title={title ?? t('title')}
				buttons={
					<>
						<Button type="submit">{t('submit')}</Button>
						{additionalButtons}
						{onCancel ? (
							<Button.Secondary onClick={onCancel}>
								{t('cancel')}
							</Button.Secondary>
						) : null}
					</>
				}
			>
				{children}
			</ModalDialogLayout>
		</form>
	);
}
