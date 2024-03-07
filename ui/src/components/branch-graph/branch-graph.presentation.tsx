import { useResizeDetector } from '../../utils/atoms/useResizeDetector';
import { FullSizeSvg } from '../svg/full-size-svg';
import { useBranchSimulation } from './branch-graph.simulation';
import { BranchLink } from './BranchLink';
import { BranchNode } from './BranchNode';
import { CenterG } from './CenterG';
import type { BranchConfiguration } from '../../generated/api/models';

export type BranchGraphPresentationProps<T extends BranchConfiguration> = {
	upstreamData: T[];
	onClick?: (node: T) => void;
};

export function BranchGraphPresentation<T extends BranchConfiguration>({
	upstreamData,
	onClick,
}: BranchGraphPresentationProps<T>) {
	const [sizeDetection, size] = useResizeDetector<SVGSVGElement>();
	const { nodes, links, restartSimulation } = useBranchSimulation(upstreamData);

	return (
		<section>
			<FullSizeSvg ref={sizeDetection}>
				<CenterG size={size}>
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
				</CenterG>
			</FullSizeSvg>
		</section>
	);
}
