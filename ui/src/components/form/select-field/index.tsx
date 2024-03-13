import { elementTemplate } from '../../templating';
import { ErrorsList } from '../errors-list';
import { Field } from '../field';
import { SelectInput } from './select-input';
import type { SelectInputProps } from './select-input';
import type { StandardField } from '../FieldProps';

export const NotSelected = elementTemplate('NotSelected', 'span', (T) => (
	<T className="text-slate-500" />
));

export function SelectField<T>(props: {
	field: StandardField<T>;
	items: readonly T[];
	children: (item: T) => React.ReactNode;
	selectInput: React.FC<SelectInputProps<T>>;
	labelContents?: React.ReactNode;
}): JSX.Element;
export function SelectField<T>(props: {
	field: StandardField<T>;
	items: readonly T[];
	children: (item: T) => React.ReactNode;
	labelContents?: React.ReactNode;
}): JSX.Element;
export function SelectField<T>({
	field,
	items,
	children,
	selectInput: InputComponent = SelectInput,
	labelContents,
}: {
	field: StandardField<T>;
	items: readonly T[];
	children: (item: T) => React.ReactNode;
	selectInput?: React.FC<SelectInputProps<T>>;
	labelContents?: React.ReactNode;
}) {
	return (
		<Field
			noLabel
			labelChildren={labelContents ?? field.translation(['label'])}
		>
			<InputComponent items={items} {...field.htmlProps.asControlled()}>
				{children}
			</InputComponent>
			<ErrorsList errors={field.errors} translations={field.translation} />
		</Field>
	);
}
