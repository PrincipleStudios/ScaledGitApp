import { useParams } from 'react-router-dom';
import {
	componentAutoBinder,
	type ComponentAutoBinder,
} from './component-auto-binder';

export function withPathParamsValue<const T extends string>(
	prop: T,
	pathParamName?: string,
): ComponentAutoBinder<T, string | undefined> {
	return componentAutoBinder(
		prop,
		function usePathParam() {
			const params = useParams();
			return params[pathParamName ?? prop];
		},
		'WithPathParams',
	);
}
