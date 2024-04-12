import { useComputedAtom } from '@principlestudios/jotai-react-signals';
import type { BranchGraphLinkDatum } from './branch-graph.simulation';

export function useComputedLinkValues(link: BranchGraphLinkDatum) {
	const position = useComputedAtom((get) => {
		const { screenX: x1, screenY: y1 = 0 } = get(link.source.atom);
		const { screenX: x2 = 0, screenY: y2 = 0 } = get(link.target.atom);
		return { x1, x2, y1, y2 };
	});
	const transform = useComputedAtom((get) => {
		const { x1, x2, y1, y2 } = get(position);
		const rad = Math.atan2(y2 - y1, x2 - x1);
		return `translate(${x2.toFixed(1)}px, ${y2.toFixed(1)}px) rotate(${rad.toFixed(4)}rad)`;
	});
	const negativeLen = useComputedAtom((get) => {
		const { x1, x2, y1, y2 } = get(position);
		const x = x2 - x1;
		const y = y2 - y1;
		return (-Math.sqrt(x * x + y * y) + 5).toFixed(1);
	});
	return { transform, negativeLen };
}
