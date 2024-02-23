import { useComputedAtom } from '@principlestudios/jotai-react-signals';
import { JotaiG, JotaiLine } from '../svg/atom-elements';
import { FullSizeSvg } from '../svg/full-size-svg';
import { useBranchSimulation } from './branch-graph.simulation';
import type {
	BranchGraphLinkDatum,
	BranchGraphNodeDatum,
	WithAtom,
} from './branch-graph.simulation';
import type { UpstreamBranches } from '../../generated/api/models';

export type BranchGraphPresentationProps = {
	upstreamData: UpstreamBranches;
};

export function BranchGraphPresentation({
	upstreamData,
}: BranchGraphPresentationProps) {
	const { nodes, links, svgRefCallback } = useBranchSimulation(upstreamData);

	return (
		<section>
			<FullSizeSvg ref={svgRefCallback}>
				{links.map((link) => (
					<BranchLink key={link.id} link={link} />
				))}
				{nodes.map((node) => (
					<BranchNode key={node.id} node={node} />
				))}
			</FullSizeSvg>
		</section>
	);
}

function BranchNode({ node }: { node: WithAtom<BranchGraphNodeDatum> }) {
	const transform = useComputedAtom((get) => {
		const { x, y } = get(node.atom);
		return `translate(${(x ?? 0).toFixed(1)}px, ${(y ?? 0).toFixed(1)}px)`;
	});
	return (
		<JotaiG style={{ transform: transform }}>
			<circle cx={0} cy={0} r={5} />
		</JotaiG>
	);
}

function BranchLink({ link }: { link: WithAtom<BranchGraphLinkDatum> }) {
	const position = useComputedAtom((get) => {
		const { x: x1 = 0, y: y1 = 0 } = get(link.source.atom);
		const { x: x2 = 0, y: y2 = 0 } = get(link.target.atom);
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
		return (-Math.sqrt(x * x + y * y)).toFixed(1);
	});
	return (
		<JotaiG style={{ transform: transform }}>
			<JotaiLine
				x1={negativeLen}
				y1={0}
				x2={0}
				y2={0}
				strokeWidth={1}
				className="stroke-black"
			/>
			<path d="M-5,0l-5,3v-6z" />
		</JotaiG>
	);
}
