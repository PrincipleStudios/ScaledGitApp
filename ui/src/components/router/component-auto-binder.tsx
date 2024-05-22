type BindableProps<TPropName extends string, TValue> = {
	[P in TPropName]?: TValue;
};

export type ComponentAutoBinder<TPropName extends string, TValue> = <
	TProps extends BindableProps<TPropName, TValue>,
>(
	Component: React.ComponentType<TProps>,
) => React.ComponentType<Omit<TProps, TPropName>>;

export function componentAutoBinder<
	const TPropName extends string,
	const TValue,
>(
	prop: TPropName,
	useValue: () => TValue,
	displayNamePrefix: string,
): ComponentAutoBinder<TPropName, TValue> {
	return <TProps extends BindableProps<TPropName, TValue>>(
		Component: React.ComponentType<TProps>,
	) => {
		function BoundComponent(props: Omit<TProps, TPropName>) {
			const value = useValue();
			return <Component {...({ ...props, [prop]: value } as TProps)} />;
		}
		BoundComponent.displayName = `${displayNamePrefix}(${prop})`;
		return BoundComponent;
	};
}
