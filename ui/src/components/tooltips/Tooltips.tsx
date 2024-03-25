import { useEffect, useMemo } from 'react';
import { subscribeToDimensionChanges } from '@/utils/atoms/subscribeToDimensionChanges';
import type { ElementDimensions } from '@/utils/atoms/useResizeDetector';
import {
	useComputedAtom,
	withSignal,
} from '@principlestudios/jotai-react-signals';
import { atom, useStore, useAtomValue } from 'jotai';
import { tooltipState } from './state';
import { TooltipContainer } from './TooltipContainer';

export function Tooltips() {
	const store = useStore();
	const current = useAtomValue(tooltipState);
	const targetElement = current?.target;
	const tooltipPosition = useMemo(() => atom<ElementDimensions>({}), []);

	useEffect(
		function () {
			return subscribeToDimensionChanges(
				targetElement ?? null,
				store,
				tooltipPosition,
			);
		},
		[targetElement, store, tooltipPosition],
	);

	const top = useComputedAtom((get) => `${get(tooltipPosition).top}px`);
	const left = useComputedAtom((get) => `${get(tooltipPosition).left}px`);

	if (!current?.contents) return null;
	return (
		<JotaiDiv className="absolute pointer-events-none" style={{ top, left }}>
			<TooltipContainer>{current.contents}</TooltipContainer>
		</JotaiDiv>
	);
}

const JotaiDiv = withSignal('div');
