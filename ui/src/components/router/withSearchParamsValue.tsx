import { useSearchParams } from 'react-router-dom';

export function withSearchParamsValue<const T extends string>(prop: T) {
	return <
		TProps extends {
			[P in T]: string[];
		},
	>(
		Component: React.ComponentType<TProps>,
	): React.ComponentType<Omit<TProps, T>> => {
		function WithParams(props: Omit<TProps, T>) {
			const [params] = useSearchParams();
			return (
				<Component {...({ ...props, [prop]: params.getAll(prop) } as TProps)} />
			);
		}
		WithParams.displayName = `WithParams(${prop})`;
		return WithParams;
	};
}
