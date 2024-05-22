export function withParam<const TPropName extends string, const TValue>(
	prop: TPropName,
	value: TValue,
) {
	return <
		TProps extends {
			[P in TPropName]: TValue;
		},
	>(
		Component: React.ComponentType<TProps>,
	): React.ComponentType<Omit<TProps, TPropName>> => {
		function WithParams(props: Omit<TProps, TPropName>) {
			return <Component {...({ ...props, [prop]: value } as TProps)} />;
		}
		WithParams.displayName = `WithParams(${prop})`;
		return WithParams;
	};
}
