export type ModalDialogLayoutProps = {
	children?: React.ReactNode;
	buttons: React.ReactNode;
	title: React.ReactNode;
};

export function ModalDialogLayout({
	children,
	buttons,
	title,
}: ModalDialogLayoutProps) {
	return (
		<>
			<div className="bg-slate-100 dark:bg-slate-900 px-4 pb-4 pt-5 sm:p-6 sm:pb-4">
				<div className="mt-3 text-center sm:ml-4 sm:mt-0 sm:text-left">
					<h3 className="text-base font-semibold leading-6 text-slate-900 dark:text-white">
						{title}
					</h3>
					<div className="flex flex-col items-stretch gap-2 mt-2">
						{children}
					</div>
				</div>
			</div>
			<div className="bg-slate-200 dark:bg-slate-800 px-4 py-3 flex flex-col sm:flex-row-reverse sm:px-6 gap-3">
				{buttons}
			</div>
		</>
	);
}
