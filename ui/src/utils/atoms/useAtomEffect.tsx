import { useEffect, useRef } from 'react';
import { useComputedAtom } from '@principlestudios/jotai-react-signals';
import type { Atom, Getter } from 'jotai';
import { useStore } from 'jotai';

export function useAtomEffect<T>(
	changeValue: Atom<T> | ((get: Getter) => T),
	onChange: (value: T) => void,
) {
	const changeRef = useRef(onChange);
	changeRef.current = onChange;
	const atom = useComputedAtom(
		typeof changeValue === 'function' ? changeValue : (get) => get(changeValue),
	);
	const store = useStore();
	useEffect(() => {
		const subscription = store.sub(atom, () => {
			changeRef.current?.(store.get(atom));
		});
		return subscription;
	}, [atom, store]);
}
