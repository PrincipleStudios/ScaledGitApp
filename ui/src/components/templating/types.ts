export type AllowedTypes =
	| keyof JSX.IntrinsicElements
	// eslint-disable-next-line @typescript-eslint/no-explicit-any
	| React.JSXElementConstructor<any>;
export type PropsOf<TType extends AllowedTypes> =
	TType extends keyof JSX.IntrinsicElements
		? JSX.IntrinsicElements[TType]
		: React.ComponentProps<TType>;

export type PartialElement<TProps> = React.JSXElementConstructor<
	Partial<TProps>
>;

export type BaseMapPropsOptions<TProps, TNewProps> = {
	/**
	 * Allows mutation of properties before passing to element.
	 *
	 * @param props The end-user's properties (or as passed in by a subsequent template)
	 * @returns The properties passed to the underlying component
	 **/
	useProps(this: void, props: TNewProps): TProps;
};

export type MapPropsOptions<TProps, TNewProps> = TProps extends TNewProps
	? TNewProps extends TProps
		? Partial<BaseMapPropsOptions<TProps, TNewProps>>
		: BaseMapPropsOptions<TProps, TNewProps>
	: BaseMapPropsOptions<TProps, TNewProps>;

export type ExtendOptions<TProps, TNewProps = TProps> = MapPropsOptions<
	TProps,
	TNewProps
> & {
	// placeholder for more options
};

export type TemplateResolver<TProps> = (
	this: void,
	template: PartialElement<TProps>,
) => React.ReactElement;

export type ThemedTemplateResolver<TKeys extends string, TProps> = Record<
	TKeys,
	TemplateResolver<TProps>
>;

export interface ElementTemplate<TProps> {
	(props: TProps): React.ReactNode;

	/** The display name of the template to React dev tools */
	displayName: string;

	/** Extends an element template to a new component that can be further extended */
	extend<TNewProps = TProps>(
		name: string,
		elem: TemplateResolver<TProps>,
		options?: ExtendOptions<TProps, TNewProps> | undefined,
	): ElementTemplate<TNewProps>;

	/**
	 * Applies a number of themes that can be accessed on the result along with
	 * the original template.
	 *
	 * @param themes A map of themes to Template resolvers. Each key becomes a
	 * new component, such that if `Button.themed({ Blue: ... })` is called,
	 * `Button.Blue` may be used.
	 **/
	themed<TKeys extends string>(
		this: ElementTemplate<TProps>,
		themes: Record<TKeys, TemplateResolver<TProps>>,
	): this & Record<TKeys, ElementTemplate<TProps>>;
}
