import { FullSizeSvg } from '../svg/full-size-svg';
import { useBranchSimulation } from './branch-graph.simulation';
import { BranchLink } from './BranchLink';
import { BranchNode } from './BranchNode';
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
