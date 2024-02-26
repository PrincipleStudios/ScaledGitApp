import { useLayoutEffect, useRef, useMemo } from 'react';
import { useSetAtom, atom } from 'jotai';
import type { PrimitiveAtom } from 'jotai';

export interface ElementDimensions {
	left?: number;
	top?: number;
	height?: number;
	width?: number;
}

// Swap out to `useEffect` in ssr scenarios, which do not currently apply
const useEnhancedEffect = useLayoutEffect;

export interface UseResizeDetectorProps<T extends HTMLElement | SVGElement> {
	targetRef?: React.RefObject<T | null | undefined>;

	/**
	 * These options will be used as a second parameter of `resizeObserver.observe` method
	 * @see https://developer.mozilla.org/en-US/docs/Web/API/ResizeObserver/observe
	 * Default: undefined
	 */
	observerOptions?: ResizeObserverOptions;
	/**
	 * The value to write the dimensions
	 */
	atom?: PrimitiveAtom<ElementDimensions>;
}

export function useResizeDetector<
	T extends HTMLElement | SVGElement = HTMLElement,
>({
	targetRef,
	observerOptions,
	atom: targetAtom,
}: UseResizeDetectorProps<T> = {}): [
	React.LegacyRef<T>,
	PrimitiveAtom<ElementDimensions>,
] {
	const localAtom = useMemo(() => atom<ElementDimensions>({}), []);
	const resultAtom = targetAtom ?? localAtom;
	const setDimensions = useSetAtom(resultAtom);

	const localRef = useRef<T | null>(null);
	const ref = targetRef ?? localRef;

	useEnhancedEffect(() => {
		const resizeCallback = () => {
			const rect = ref.current?.getBoundingClientRect();
			if (!rect) return;
			setDimensions(rect);
		};

		const resizeObserver = new window.ResizeObserver(resizeCallback);

		if (ref.current) {
			resizeObserver.observe(ref.current, observerOptions);
		}
		resizeCallback();

		return () => {
			resizeObserver.disconnect();
		};
	}, [observerOptions, ref.current]);

	return [ref as React.LegacyRef<T>, resultAtom];
}
