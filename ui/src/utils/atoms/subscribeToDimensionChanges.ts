import type { PrimitiveAtom } from 'jotai';
import type { JotaiStore } from './JotaiStore';
import type { ElementDimensions } from './useResizeDetector';

export function subscribeToDimensionChanges<
	T extends HTMLElement | SVGElement = HTMLElement,
>(
	elem: T | null,
	store: JotaiStore,
	atom: PrimitiveAtom<ElementDimensions>,
	observerOptions?: ResizeObserverOptions | undefined,
) {
	const resizeCallback = () => {
		const rect = elem?.getBoundingClientRect();
		if (!rect) return;
		store.set(atom, rect);
	};

	const resizeObserver = new window.ResizeObserver(resizeCallback);

	if (elem) {
		resizeObserver.observe(elem, observerOptions);
	}
	resizeCallback();

	return () => {
		resizeObserver.disconnect();
	};
}
