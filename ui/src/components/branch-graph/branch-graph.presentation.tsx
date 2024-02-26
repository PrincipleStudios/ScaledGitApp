import { useResizeDetector } from '../../utils/atoms/useResizeDetector';
import { FullSizeSvg } from '../svg/full-size-svg';
import { useBranchSimulation } from './branch-graph.simulation';
import { BranchLink } from './BranchLink';
import { BranchNode } from './BranchNode';
import { CenterG } from './CenterG';
import type { UpstreamBranches } from '../../generated/api/models';

export type BranchGraphPresentationProps = {
	upstreamData: UpstreamBranches;
};

export function BranchGraphPresentation({
	upstreamData,
}: BranchGraphPresentationProps) {
	const [sizeDetection, size] = useResizeDetector<SVGSVGElement>();
	const { nodes, links } = useBranchSimulation(upstreamData);

	return (
		<section>
			<FullSizeSvg ref={sizeDetection}>
				<CenterG size={size}>
					{links.map((link) => (
						<BranchLink key={link.id} link={link} />
					))}
					{nodes.map((node) => (
						<BranchNode key={node.id} node={node} />
					))}
				</CenterG>
			</FullSizeSvg>
		</section>
	);
}
