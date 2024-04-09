import { useComputedAtom } from '@principlestudios/jotai-react-signals';
import { setupDragHandler } from '@/utils/dragging';
import { BranchSvgCircle } from '../branch-display';
import { JotaiG } from '../svg/atom-elements';
import type { BranchGraphNodeDatum, WithAtom } from './branch-graph.simulation';

export function BranchNode({
	node,
	onMove,
	onClick,
}: {
	node: WithAtom<BranchGraphNodeDatum>;
	onMove?: () => void;
	onClick?: () => void;
}) {
	const transform = useComputedAtom((get) => {
		const { screenX, screenY } = get(node.atom);
		return `translate(${screenX.toFixed(1)}px, ${screenY.toFixed(1)}px)`;
	});
	return (
		<JotaiG style={{ transform: transform }} {...drag(node, onMove, onClick)}>
			<BranchSvgCircle data={node.data} />
		</JotaiG>
	);
}

function drag(
	node: WithAtom<BranchGraphNodeDatum>,
	onMove?: () => void,
	onClick?: () => void,
) {
	// Updates the node's position based on mouse movements.
	// `fx`/`fy` are fixed positions - use them while moving them
	// `x`/`y` are positions that can be moved by the simulation
	return setupDragHandler({
		onMouseDown(ev) {
			ev.preventDefault();
			return {
				xOffset: (node.fx ?? node.x ?? 0) - ev.clientX,
				yOffset: (node.fy ?? node.y ?? 0) - ev.clientY,
			};
		},
		onMouseMove(ev, { xOffset, yOffset }, { totalMovement }) {
			if (totalMovement > 5) {
				node.fx = xOffset + ev.clientX;
				node.fy = yOffset + ev.clientY;
				onMove?.();
			}
		},
		onMouseUp(ev, { xOffset, yOffset }, { totalMovement }) {
			if (totalMovement <= 5) {
				onClick?.();
			} else {
				if (ev.button === 0) {
					node.y = yOffset + ev.clientY;
					node.x = xOffset + ev.clientX;
					node.fy = null;
					node.fx = null;
				}
				onMove?.();
			}
		},
	});
}
