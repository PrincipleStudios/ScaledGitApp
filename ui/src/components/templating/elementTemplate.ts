import type React from 'react';
import { createElement, forwardRef } from 'react';
import { twMerge } from 'tailwind-merge';
import type {
	AllowedTypes,
	ElementTemplate,
	ExtendOptions,
	MapPropsOptions,
	PartialElement,
	PropsOf,
	TemplateResolver,
} from './types';
import { identity } from './identity';

export function elementTemplate<TType extends AllowedTypes>(
	name: string,
	originalType: TType,
	templateResolver: TemplateResolver<PropsOf<TType>>,
	options?: ExtendOptions<PropsOf<TType>, PropsOf<TType>>,
): ElementTemplate<PropsOf<TType>>;
export function elementTemplate<
	TType extends AllowedTypes,
	TBaseProps = PropsOf<TType>,
>(
	name: string,
	originalType: TType,
	templateResolver: TemplateResolver<PropsOf<TType>>,
	options: ExtendOptions<PropsOf<TType>, TBaseProps>,
): ElementTemplate<TBaseProps>;
/**
 * Creates a new React component from a given template, allowing further
 * extensions and theming. This accounts for using `tailwind-merge` to merge
 * classes together (latest-wins) and style elements together (also
 * latest-wins). Also includes forwardRef into components that support it.
 *
 * @param name Display name of the component in React Dev Tools
 * @param originalType The original component to wrap. May be either an
 * intrinsic element such as 'div' or 'button' or another React component.
 * @param templateResolver The template function with default fields. A
 * component type is passed in to support the original component. If another
 * component is used, the new component type will be used.
 * @param options Optional. @see ExtendOptions for more details on each
 * property.
 **/
export function elementTemplate<
	TType extends AllowedTypes,
	TBaseProps = PropsOf<TType>,
>(
	name: string,
	originalType: TType,
	templateResolver: TemplateResolver<PropsOf<TType>>,
	options: Partial<ExtendOptions<PropsOf<TType>, TBaseProps>> = {},
): ElementTemplate<TBaseProps> {
	type TProps = PropsOf<TType>;
	const {
		type,
		props: {
			className: defaultClassName,
			style: defaultStyle,
			...defaultProps
		},
	} = templateResolver(originalType as PartialElement<TProps>);
	const useProps: (props: TBaseProps) => TProps = options?.useProps ?? identity;
	const base = forwardRef(
		({ children, className, style, ...props }: TProps, ref) =>
			createElement(
				type,
				useProps({
					...defaultProps,
					...props,
					className: twMerge(defaultClassName as string, className as string),
					style: { ...defaultStyle, ...style },
					ref,
				} as TProps),
				children as React.ReactNode,
			),
	) as React.FC<TBaseProps>;
	return Object.assign(base, {
		displayName: name,
		extend: <TNewProps>(
			name: string,
			templateResolver: TemplateResolver<TBaseProps>,
			extendedOptions?: ExtendOptions<TBaseProps, TNewProps> | undefined,
		): ElementTemplate<TNewProps> => {
			const {
				type: myType,
				props: { className, style, ...props },
			} = templateResolver(type as PartialElement<TBaseProps>);
			const useNewProps =
				extendedOptions?.useProps ??
				(identity as (props: TNewProps) => TBaseProps);
			return elementTemplate<TType, TNewProps>(
				name,
				myType as TType,
				(T) =>
					createElement(T, {
						...defaultProps,
						...props,
						className: twMerge(defaultClassName as string, className as string),
						style: { ...defaultStyle, ...style },
					} as TProps),
				{
					useProps: (orig) =>
						// eslint-disable-next-line @typescript-eslint/no-unsafe-return
						useProps(useNewProps(orig)),
				} as MapPropsOptions<TProps, TNewProps>,
			);
		},
		themed<TKeys extends string>(
			this: ElementTemplate<TBaseProps>,
			themes: Record<TKeys, TemplateResolver<TBaseProps>>,
		): ElementTemplate<TBaseProps> &
			Record<TKeys, ElementTemplate<TBaseProps>> {
			const result = Object.assign(
				this,
				Object.fromEntries(
					Object.entries<TemplateResolver<TBaseProps>>(themes)
						.filter(([, example]) => typeof example === 'function')
						.map(
							([name, example]) =>
								[
									name as TKeys,
									this.extend(`${this.displayName}.${name}`, example),
								] as const,
						),
				) as Record<TKeys, ElementTemplate<TBaseProps>>,
			);
			return result;
		},
	});
}
