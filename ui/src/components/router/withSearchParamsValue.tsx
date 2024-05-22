import { useSearchParams } from 'react-router-dom';
import type { ComponentAutoBinder } from './component-auto-binder';
import { componentAutoBinder } from './component-auto-binder';

export function withSearchParamsValue<const T extends string>(
	prop: T,
): ComponentAutoBinder<T, string[]> {
	return componentAutoBinder(
		prop,
		function useSearchParam() {
			const [params] = useSearchParams();
			return params.getAll(prop);
		},
		'WithParams',
	);
}
