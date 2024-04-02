import type { TFunction } from 'i18next';
import { elementTemplate } from '../../templating';
import { ErrorsList } from '../errors-list';
import { Field } from '../field';
import type { StandardField } from '../FieldProps';
import { translateField } from '../utils/translations';
import { SelectInput } from './select-input';
import type { SelectInputProps } from './select-input';

export const NotSelected = elementTemplate('NotSelected', 'span', (T) => (
	<T className="text-slate-500" />
));

export function SelectField<T>(props: {
	field: StandardField<T>;
	translation: TFunction;
	items: readonly T[];
	children: (item: T) => React.ReactNode;
	selectInput: React.FC<SelectInputProps<T>>;
	labelContents?: React.ReactNode;
}): JSX.Element;
export function SelectField<T>(props: {
	field: StandardField<T>;
	translation: TFunction;
	items: readonly T[];
	children: (item: T) => React.ReactNode;
	labelContents?: React.ReactNode;
}): JSX.Element;
export function SelectField<T>({
	field,
	translation,
	items,
	children,
	selectInput: InputComponent = SelectInput,
	labelContents,
}: {
	field: StandardField<T>;
	translation: TFunction;
	items: readonly T[];
	children: (item: T) => React.ReactNode;
	selectInput?: React.FC<SelectInputProps<T>>;
	labelContents?: React.ReactNode;
}) {
	const t = translateField(field, translation);
	return (
		<Field noLabel labelChildren={labelContents ?? t(['label'])}>
			<InputComponent items={items} {...field.htmlProps.asControlled()}>
				{children}
			</InputComponent>
			<ErrorsList errors={field.errors} translations={t} />
		</Field>
	);
}
