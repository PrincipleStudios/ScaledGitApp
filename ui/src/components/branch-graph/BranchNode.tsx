import { useComputedAtom } from '@principlestudios/jotai-react-signals';
import { JotaiG } from '../svg/atom-elements';
import { useTooltipReference } from '../tooltips';
import type { BranchGraphNodeDatum, WithAtom } from './branch-graph.simulation';

export function BranchNode({ node }: { node: WithAtom<BranchGraphNodeDatum> }) {
	const transform = useComputedAtom((get) => {
		const { x, y } = get(node.atom);
		return `translate(${(x ?? 0).toFixed(1)}px, ${(y ?? 0).toFixed(1)}px)`;
	});
	const tooltip = useTooltipReference(() => (
		<span className="text-nowrap">{node.id}</span>
	));
	return (
		<JotaiG style={{ transform: transform, fill: node.color }} {...tooltip()}>
			<circle cx={0} cy={0} r={5} />
		</JotaiG>
	);
}
