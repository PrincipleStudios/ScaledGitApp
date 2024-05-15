/* eslint-disable jsx-a11y/no-static-element-interactions,jsx-a11y/click-events-have-key-events */
// Click events and key events are handled via a global handler in Modals with
// the escape key.
import { Fragment, forwardRef, useState } from 'react';
import { Transition } from '@headlessui/react';
import { useAsAtom } from '@principlestudios/jotai-react-signals';
import type { Atom } from 'jotai';
import { useAtomValue } from 'jotai';

function FullPageModalContainer({
	children,
	label,
	show,
	onReadyToUnmount,
}: {
	children: React.ReactNode;
	label: string;
	show?: Atom<boolean>;
	onReadyToUnmount?: () => void;
}) {
	const [isShowing, setIsShowing] = useState(false);
	const shouldShow = useAtomValue(useAsAtom(show ?? true));
	if (shouldShow != isShowing) setTimeout(() => setIsShowing(shouldShow), 0);

	return (
		<Transition
			appear
			show={isShowing}
			as={Fragment}
			afterLeave={onReadyToUnmount}
		>
			<div
				className="relative z-modalBackground"
				aria-label={label}
				role="dialog"
				aria-modal="true"
			>
				{children}
			</div>
		</Transition>
	);
}

function ModalBackdrop() {
	return (
		<Transition.Child
			as={Fragment}
			enter="ease-out duration-300"
			enterFrom="opacity-0"
			enterTo="opacity-100"
			leave="ease-in duration-200"
			leaveFrom="opacity-100"
			leaveTo="opacity-0"
		>
			<div className="fixed inset-0 bg-black bg-opacity-25 transition-opacity" />
		</Transition.Child>
	);
}

const ModalPanel = forwardRef(function ModalPanel(
	{
		children,
		onCancel,
	}: {
		children: React.ReactNode;
		onCancel?: () => void;
	},
	ref: React.ForwardedRef<HTMLDivElement>,
) {
	return (
		<div className="fixed inset-0 z-modalForeground overflow-y-auto" ref={ref}>
			<div
				className="flex min-h-full items-end justify-center p-4 text-center sm:items-center sm:p-0"
				onClick={(ev) => {
					ev.preventDefault();
					ev.stopPropagation();
					if (ev.currentTarget !== ev.target) {
						return;
					}
					onCancel?.();
				}}
			>
				<Transition.Child
					as={Fragment}
					enter="ease-out duration-300"
					enterFrom="opacity-0 translate-y-4 sm:translate-y-0 sm:scale-95"
					enterTo="opacity-100 translate-y-0 sm:scale-100"
					leave="ease-in duration-200"
					leaveFrom="opacity-100 translate-y-0 sm:scale-100"
					leaveTo="opacity-0 translate-y-4 sm:translate-y-0 sm:scale-95"
				>
					<div
						className="relative transform bg-slate-100 dark:bg-slate-900 text-left shadow-xl transition-all sm:my-8 sm:w-full sm:max-w-lg"
						onClick={(ev) => ev.stopPropagation()}
					>
						{children}
					</div>
				</Transition.Child>
			</div>
		</div>
	);
});

export function Modal({
	children,
	show,
	label,
	onBackdropCancel,
	onReadyToUnmount,
}: {
	children: React.ReactNode;
	show?: Atom<boolean>;
	label: string;
	onReadyToUnmount?: () => void;
	onBackdropCancel?: () => void;
}) {
	return (
		<FullPageModalContainer
			label={label}
			show={show}
			onReadyToUnmount={onReadyToUnmount}
		>
			<ModalBackdrop />

			<ModalPanel onCancel={onBackdropCancel}>{children}</ModalPanel>
		</FullPageModalContainer>
	);
}
