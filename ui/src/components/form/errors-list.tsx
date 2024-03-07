import type {
	ErrorsAtom,
	FieldTranslation,
} from '@principlestudios/react-jotai-forms';
import { useAtomValue } from 'jotai';
import { HiX } from 'react-icons/hi';
import { elementTemplate } from '../templating';

export function ErrorsList({
	errors,
	translations,
}: {
	errors: ErrorsAtom;
	translations: FieldTranslation;
}) {
	const errorsValue = useAtomValue(errors);
	if (errorsValue.state !== 'hasData' || !errorsValue.data) return null;
	return (
		<ul className="text-red-800 dark:text-red-500 font-bold text-xs">
			{errorsValue.data.issues.map((issue, key) => (
				<ErrorsListItem key={key}>
					{translations(['errors', issue.code])}
				</ErrorsListItem>
			))}
		</ul>
	);
}

function ErrorsListItem({ children }: { children?: React.ReactNode }) {
	return (
		<li>
			<HiX className="inline-block mb-1 mr-1" />
			{children}
		</li>
	);
}

export const ErrorsListPresentation = Object.assign(
	elementTemplate('ErrorsListPresentation', 'ul', (T) => (
		<T className="text-red-800 dark:text-red-500 font-bold text-xs" />
	)),
	{
		Item: elementTemplate(
			'ErrorsListPresentation.Item',
			ErrorsListItem,
			(T) => <T />,
		),
	},
);
