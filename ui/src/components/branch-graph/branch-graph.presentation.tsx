import type { Branch, BranchConfiguration } from '@/generated/api/models';
import { useResizeDetector } from '@/utils/atoms/useResizeDetector';
import { FullSizeSvg } from '../svg/full-size-svg';
import { useBranchSimulation } from './branch-graph.simulation';
import { BranchLink } from './BranchLink';
import { BranchNode } from './BranchNode';

export type BranchGraphPresentationProps = {
	upstreamData: BranchConfiguration[];
	onClick?: (node: Branch) => void;
};

export function BranchGraphPresentation({
	upstreamData,
	onClick,
}: BranchGraphPresentationProps) {
	const [sizeDetection, size] = useResizeDetector<SVGSVGElement>();
	const { nodes, links, restartSimulation } = useBranchSimulation(
		upstreamData,
		size,
	);

	return (
		<section>
			<FullSizeSvg ref={sizeDetection}>
				{links.map((link) => (
					<BranchLink key={link.id} link={link} />
				))}
				{nodes.map((node) => (
					<BranchNode
						key={node.id}
						node={node}
						onMove={restartSimulation}
						onClick={onClick && (() => onClick(node.data))}
					/>
				))}
			</FullSizeSvg>
		</section>
	);
}
