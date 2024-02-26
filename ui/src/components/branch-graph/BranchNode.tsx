import { useComputedAtom } from '@principlestudios/jotai-react-signals';
import { JotaiG } from '../svg/atom-elements';
import type { BranchGraphNodeDatum, WithAtom } from './branch-graph.simulation';

export function BranchNode({ node }: { node: WithAtom<BranchGraphNodeDatum> }) {
	const transform = useComputedAtom((get) => {
		const { x, y } = get(node.atom);
		return `translate(${(x ?? 0).toFixed(1)}px, ${(y ?? 0).toFixed(1)}px)`;
	});
	return (
		<JotaiG style={{ transform: transform }}>
			<circle cx={0} cy={0} r={5} />
			<text>{node.depth}</text>
		</JotaiG>
	);
}
