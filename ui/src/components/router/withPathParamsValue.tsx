import { useParams } from 'react-router-dom';

export function withPathParamsValue<const T extends string>(
	prop: T,
	pathParamName?: string,
) {
	return <
		TProps extends {
			[P in T]?: string | undefined;
		},
	>(
		Component: React.ComponentType<TProps>,
	): React.ComponentType<Omit<TProps, T>> => {
		function WithParams(props: Omit<TProps, T>) {
			const params = useParams();
			return (
				<Component
					{...({ ...props, [prop]: params[pathParamName ?? prop] } as TProps)}
				/>
			);
		}
		WithParams.displayName = `WithPathParams(${prop})`;
		return WithParams;
	};
}
