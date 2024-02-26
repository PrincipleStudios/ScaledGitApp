import { useComputedAtom } from '@principlestudios/jotai-react-signals';
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

function drag(
	node: WithAtom<BranchGraphNodeDatum>,
	onMove?: () => void,
): Pick<
	JSX.IntrinsicElements['g'],
	'onMouseDown' | 'onMouseMove' | 'onMouseUp'
> {
	let yOffset: number | null = null;
	let xOffset: number | null = null;
	return {
		onMouseDown(ev) {
			yOffset = (node.fy ?? node.y ?? 0) - ev.clientY;
			xOffset = (node.fx ?? node.x ?? 0) - ev.clientX;
			document.addEventListener('mousemove', onMouseMove, true);
			document.addEventListener('mouseup', onMouseUp, true);
		},
	};

	function onMouseMove(ev: MouseEvent) {
		if (yOffset === null || xOffset === null) return;
		node.fy = yOffset + ev.clientY;
		node.fx = xOffset + ev.clientX;
		onMove?.();
	}

	function onMouseUp(ev: MouseEvent) {
		document.removeEventListener('mousemove', onMouseMove, true);
		document.removeEventListener('mouseup', onMouseUp, true);
		if (yOffset === null || xOffset === null) return;
		node.y = yOffset + ev.clientY;
		node.fy = null;
		node.fx = null;
		yOffset = null;
		xOffset = null;
		onMove?.();
	}
}
