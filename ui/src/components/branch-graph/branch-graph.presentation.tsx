import { useComputedAtom } from '@principlestudios/jotai-react-signals';
import { JotaiCircle, JotaiLine } from '../svg/atom-elements';
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
	const x = useComputedAtom((get) => get(node.atom).x ?? 0);
	const y = useComputedAtom((get) => get(node.atom).y ?? 0);
	return <JotaiCircle cx={x} cy={y} r={5} />;
}

function BranchLink({ link }: { link: WithAtom<BranchGraphLinkDatum> }) {
	const x1 = useComputedAtom((get) => get(link.source.atom).x ?? 0);
	const y1 = useComputedAtom((get) => get(link.source.atom).y ?? 0);
	const x2 = useComputedAtom((get) => get(link.target.atom).x ?? 0);
	const y2 = useComputedAtom((get) => get(link.target.atom).y ?? 0);
	return (
		<JotaiLine
			x1={x1}
			y1={y1}
			x2={x2}
			y2={y2}
			strokeWidth={1}
			className="stroke-black"
		/>
	);
}
