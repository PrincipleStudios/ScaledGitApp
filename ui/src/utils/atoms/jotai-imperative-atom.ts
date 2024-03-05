import { type Atom, atom, type Getter } from 'jotai';
import type { JotaiStore } from './JotaiStore';

function atomWithRefresh<T extends object>(fn: (get: Getter) => T) {
	const refreshCounter = atom(0);

	return atom(
		(get) => {
			get(refreshCounter);
			return { ...fn(get) };
		},
		(get, set) => {
			set(refreshCounter, (i) => i + 1);
		},
	);
}

export function atomWithImperativeProxy<T extends object>(
	targetValue: T,
	store: JotaiStore,
): readonly [proxy: T, atom: Atom<T>] {
	const onChange = atomWithRefresh(() => targetValue);

	const handler: ProxyHandler<object> = {
		get(target, property, receiver) {
			try {
				return target[property as keyof typeof target];
			} catch (err) {
				// eslint-disable-next-line @typescript-eslint/no-unsafe-return
				return Reflect.get(target, property, receiver);
			}
		},
		defineProperty(target, property, descriptor) {
			const result = Reflect.defineProperty(target, property, descriptor);
			store.set(onChange);

			return result;
		},
		deleteProperty(target, property) {
			const result = Reflect.deleteProperty(target, property);
			store.set(onChange);

			return result;
		},
	};

	return [new Proxy<T>(targetValue, handler), onChange] as const;
}
