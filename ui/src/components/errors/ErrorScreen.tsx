import { useTranslation } from 'react-i18next';
import { HiXCircle } from 'react-icons/hi2';
import { elementTemplate } from '../templating';
import { LargeInstructions, Prose } from '../text';

type ErrorScreenProps = {
	message: string;
	explanation?: string;
};

const Flex1 = elementTemplate('Flex1', 'div', (T) => <T className="flex-1" />);
const Flex2 = elementTemplate('Flex1', 'div', (T) => (
	<T className="flex-[2]" />
));
const Centered = elementTemplate('Centered', 'div', (T) => (
	<T className="w-full h-full flex flex-col items-center" />
));

export function ErrorScreen({ message, explanation }: ErrorScreenProps) {
	return (
		<Centered className="px-4">
			<Flex1 />
			<LargeInstructions className="text-red-800 dark:text-red-200">
				<HiXCircle className="inline-block" /> {message}
			</LargeInstructions>
			{explanation && <Prose className="my-4 max-w-lg">{explanation}</Prose>}
			<Flex2 />
		</Centered>
	);
}

ErrorScreen.Icon = function ({ message, explanation }) {
	return (
		<Centered className="justify-center text-4xl text-red-800 dark:text-red-200">
			<HiXCircle title={explanation ? `${message}. ${explanation}` : message} />
		</Centered>
	);
} as React.FC<ErrorScreenProps>;
ErrorScreen.Icon.displayName = 'ErrorScreen.Icon';

ErrorScreen.Widget = function ({ message, explanation }) {
	return (
		<Centered
			className="justify-center text-2xl border rounded-md border-red-600 bg-red-50 dark:bg-red-950"
			title={explanation}
		>
			<HiXCircle className="text-red-800 dark:text-red-200" />
			<Prose>{message}</Prose>
		</Centered>
	);
} as React.FC<ErrorScreenProps>;
ErrorScreen.Widget.displayName = 'ErrorScreen.Widget';

function sized<TProps>(
	name: string,
	target: React.FC<TProps> & Record<'Icon' | 'Widget', React.FC<TProps>>,
) {
	const result: React.FC<TProps & { size: 'icon' | 'widget' | 'screen' }> = ({
		size,
		...props
	}) => {
		const Component =
			size === 'icon'
				? target.Icon
				: size === 'widget'
					? target.Widget
					: target;
		return <Component {...(props as JSX.IntrinsicAttributes & TProps)} />;
	};
	result.displayName = name;
	return result;
}
ErrorScreen.Sized = sized('ErrorScreen.Sized', ErrorScreen);

type TranslatedProps = {
	namespace: string;
};

const Translated = elementTemplate<typeof ErrorScreen, TranslatedProps>(
	'Translated',
	ErrorScreen,
	(T) => <T />,
	{
		useProps({ namespace }) {
			const { t } = useTranslation(namespace);
			return { message: t('message'), explanation: t('explanation') };
		},
	},
);

ErrorScreen.translated = function translated(
	name: string,
	{ namespace }: TranslatedProps,
) {
	// eslint-disable-next-line @typescript-eslint/ban-types
	const result = Translated.extend<{}>(name, (T) => <T />, {
		useProps: () => ({ namespace }),
	});
	const withTheme = result.themed({
		// @ts-expect-error partial widget for template
		Icon: () => <ErrorScreen.Icon />,
		// @ts-expect-error partial widget for template
		Widget: () => <ErrorScreen.Widget />,
	});
	return Object.assign(withTheme, {
		Sized: sized(`${name}.Sized`, withTheme),
	});
};
