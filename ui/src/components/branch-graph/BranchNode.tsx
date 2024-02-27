import { useComputedAtom } from '@principlestudios/jotai-react-signals';
import { setupDragHandler } from '../../utils/dragging';
import { JotaiG } from '../svg/atom-elements';
import { useTooltipReference } from '../tooltips';
import type { BranchGraphNodeDatum, WithAtom } from './branch-graph.simulation';

export function BranchNode({
	node,
	onMove,
}: {
	node: WithAtom<BranchGraphNodeDatum>;
	onMove?: () => void;
}) {
	const transform = useComputedAtom((get) => {
		const { x, y } = get(node.atom);
		return `translate(${(x ?? 0).toFixed(1)}px, ${(y ?? 0).toFixed(1)}px)`;
	});
	const tooltip = useTooltipReference(() => (
		<span className="text-nowrap">{node.id}</span>
	));
	return (
		<JotaiG
			style={{ transform: transform, fill: node.color }}
			{...tooltip()}
			{...drag(node, onMove)}
		>
			<circle cx={0} cy={0} r={5} />
		</JotaiG>
	);
}

function drag(node: WithAtom<BranchGraphNodeDatum>, onMove?: () => void) {
	// Updates the node's position based on mouse movements.
	// `fx`/`fy` are fixed positions - use them while moving them
	// `x`/`y` are positions that can be moved by the simulation
	return setupDragHandler({
		onMouseDown(ev) {
			return {
				xOffset: (node.fx ?? node.x ?? 0) - ev.clientX,
				yOffset: (node.fy ?? node.y ?? 0) - ev.clientY,
			};
		},
		onMouseMove(ev, { xOffset, yOffset }) {
			node.fx = xOffset + ev.clientX;
			node.fy = yOffset + ev.clientY;
			onMove?.();
		},
		onMouseUp(ev, { xOffset, yOffset }) {
			node.y = yOffset + ev.clientY;
			node.x = xOffset + ev.clientX;
			node.fy = null;
			node.fx = null;
			onMove?.();
		},
	});
}
