import { translateField } from '@/utils/translations';
import { useComputedAtom } from '@principlestudios/jotai-react-signals';
import { useTwMerge } from '../../jotai/useTwMerge';
import { elementTemplate } from '../../templating';
import { ErrorsList } from '../errors-list';
import { Field } from '../field';
import { TextInput } from './text-input';
import type { JotaiLabel } from '../../jotai/label';
import type { FieldProps } from '../FieldProps';

export type TextFieldPersistentProps = {
	description?: boolean;
	type?: React.HTMLInputTypeAttribute;
	labelClassName?: string;
	inputClassName?: string;
	contentsClassName?: string;
} & React.ComponentProps<typeof JotaiLabel>;
export type TextFieldProps = FieldProps<string> & TextFieldPersistentProps;

function BaseTextField(props: TextFieldProps) {
	const htmlProps = props.field.htmlProps();
	const {
		field: { errors },
		translation,
		type,
		description,
		labelClassName,
		inputClassName,
		contentsClassName,
		...fieldProps
	} = props;
	const disabledLabelClassName = useTwMerge(
		useComputedAtom((get) => (get(htmlProps.disabled) ? 'text-slate-500' : '')),
		labelClassName,
	);
	const t = translateField(props.field, translation);
	return (
		<Field
			{...fieldProps}
			labelClassName={disabledLabelClassName}
			labelChildren={t(['label'])}
			contentsClassName={contentsClassName}
		>
			<TextInput type={type} {...htmlProps} className={inputClassName} />
			{description && <p className="text-xs italic">{t(['description'])}</p>}
			<ErrorsList errors={errors} translations={t} />
		</Field>
	);
}

export const TextField = elementTemplate(
	'TextField',
	BaseTextField as React.FC<TextFieldProps>,
	(T) => <T />,
);
