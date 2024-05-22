import {
	componentAutoBinder,
	type ComponentAutoBinder,
} from './component-auto-binder';

export function withParam<const TPropName extends string, const TValue>(
	prop: TPropName,
	value: TValue,
): ComponentAutoBinder<TPropName, TValue> {
	return componentAutoBinder(prop, () => value, 'WithParams');
}
