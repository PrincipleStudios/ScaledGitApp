import { useTranslation } from 'react-i18next';
import { useComputedAtom } from '@principlestudios/jotai-react-signals';
import { setupDragHandler } from '../../utils/dragging';
import { TooltipLine } from '../common';
import { JotaiG } from '../svg/atom-elements';
import { useTooltipReference } from '../tooltips';
import type { BranchGraphNodeDatum, WithAtom } from './branch-graph.simulation';
import type {
	BranchConfiguration,
	BranchDetails,
} from '../../generated/api/models';

function isDetailed(
	branch: BranchConfiguration | BranchDetails,
): branch is BranchDetails {
	return branch ? 'nonMergeCommitCount' in branch : false;
}

type NodeDetails = BranchConfiguration | BranchDetails;

export function BranchNode({
	node,
	onMove,
	onClick,
}: {
	node: WithAtom<BranchGraphNodeDatum<NodeDetails>>;
	onMove?: () => void;
	onClick?: () => void;
}) {
	const transform = useComputedAtom((get) => {
		const { x, y } = get(node.atom);
		return `translate(${(x ?? 0).toFixed(1)}px, ${(y ?? 0).toFixed(1)}px)`;
	});
	const details = isDetailed(node.data)
		? node.data
		: { ...node.data, nonMergeCommitCount: 1, exists: true };
	const tooltip = useTooltipReference(() => (
		<BranchNodeTooltip details={node.data} />
	));
	return (
		<JotaiG
			style={{
				transform: transform,
				fill: details.exists ? node.data.color : 'transparent',
				opacity: details.nonMergeCommitCount > 0 ? 1 : 0.5,
				stroke: node.data.color,
			}}
			{...tooltip()}
			{...drag(node, onMove, onClick)}
		>
			<circle cx={0} cy={0} r={5} />
		</JotaiG>
	);
}

function BranchNodeTooltip({ details }: { details: NodeDetails }) {
	const { t } = useTranslation('branch-graph');
	if (!isDetailed(details))
		return <span className="text-nowrap">{details.name}</span>;
	return (
		<>
			<TooltipLine>{details.name}</TooltipLine>
			{!details.exists ? (
				<TooltipLine>{t('does-not-exist')}</TooltipLine>
			) : null}
			<TooltipLine>
				{t('commits')}: {details.nonMergeCommitCount}
			</TooltipLine>
		</>
	);
}

function drag(
	node: WithAtom<BranchGraphNodeDatum<BranchConfiguration | BranchDetails>>,
	onMove?: () => void,
	onClick?: () => void,
) {
	// Updates the node's position based on mouse movements.
	// `fx`/`fy` are fixed positions - use them while moving them
	// `x`/`y` are positions that can be moved by the simulation
	return setupDragHandler({
		onMouseDown(ev) {
			ev.preventDefault();
			return {
				xOffset: (node.fx ?? node.x ?? 0) - ev.clientX,
				yOffset: (node.fy ?? node.y ?? 0) - ev.clientY,
			};
		},
		onMouseMove(ev, { xOffset, yOffset }, { totalMovement }) {
			if (totalMovement > 5) {
				node.fx = xOffset + ev.clientX;
				node.fy = yOffset + ev.clientY;
				onMove?.();
			}
		},
		onMouseUp(ev, { xOffset, yOffset }, { totalMovement }) {
			if (totalMovement <= 5) {
				onClick?.();
			} else {
				if (ev.button === 0) {
					node.y = yOffset + ev.clientY;
					node.x = xOffset + ev.clientX;
					node.fy = null;
					node.fx = null;
				}
				onMove?.();
			}
		},
	});
}
