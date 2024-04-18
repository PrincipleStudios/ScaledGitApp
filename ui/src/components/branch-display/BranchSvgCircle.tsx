import {
	useComputedAtom,
	type CSSPropertiesWithSignal,
} from '@principlestudios/jotai-react-signals';
import { useQueryClient } from '@tanstack/react-query';
import type { BranchDetails } from '@/generated/api/models';
import { queries } from '@/utils/api/queries';
import { JotaiCircle } from '../svg/atom-elements';
import { useTooltipReference } from '../tooltips';
import { activeBranchNames } from './active';
import { BranchNodeTooltip } from './BranchNodeTooltip';
import { useGraphSvgStyleContext } from './BranchSvgDefs';
import { isDetailed, type BranchInfo, isBasic } from './types';
import { useActiveBranchOnHover } from './useActiveBranchOnHover';

export const branchNodeRadius = 5;
const smallBranchNodeRadius = 5;
export const branchNodeStrokeWidth = 1;
export const branchNodeRadiusWithStroke =
	branchNodeRadius + branchNodeStrokeWidth / 2;

export function getBranchNodeRadius(branch: BranchInfo) {
	return isDetailed(branch) ? branchNodeRadius : smallBranchNodeRadius;
}

/** A node in a branch graph to represent a branch. Centers branch at 0,0 - use
 * transforms outside of this item to position elsewhere. */
export function BranchSvgCircle({ data }: { data: BranchInfo }) {
	const queryClient = useQueryClient();
	if (!isDetailed(data)) {
		const cachedDetails = queryClient.getQueryState(
			queries.getBranchDetails(data.name).queryKey,
		)?.data as BranchDetails;
		if (cachedDetails) data = cachedDetails;
	}
	const tooltip = useTooltipReference(() => (
		<BranchNodeTooltip details={data} />
	));
	const contextStyles = useGraphSvgStyleContext();
	const outerStyles: React.CSSProperties = {
		color: data.color,
	};
	const strokeStyles: React.CSSProperties = {
		fill: 'none',
		strokeDasharray: isBasic(data) ? undefined : '3,3',
		stroke: 'currentcolor',
		strokeWidth: branchNodeStrokeWidth,
	};
	const fillStyles: React.CSSProperties = {
		fill: isDetailed(data) && !data.exists ? 'transparent' : 'currentcolor',
		fillOpacity: data.color ? 1 : 0.5,
		opacity: isBasic(data) ? 1 : 0.25,
	};
	const isActive = useComputedAtom((get) =>
		get(activeBranchNames).includes(data.name),
	);
	const focusStyles: CSSPropertiesWithSignal = {
		...strokeStyles,
		opacity: useComputedAtom((get) => (get(isActive) ? '1' : '0')),
		transform: useComputedAtom((get) =>
			get(isActive) ? 'scale(1.4)' : 'scale(1.0)',
		),
	};
	const onHover = useActiveBranchOnHover(data);

	const fillAttrs =
		isDetailed(data) && data.nonMergeCommitCount === 0
			? contextStyles.noCommits
			: {};

	return (
		<g {...tooltip()}>
			<g {...onHover} style={outerStyles}>
				<circle
					cx={0}
					cy={0}
					r={getBranchNodeRadius(data)}
					style={fillStyles}
					{...fillAttrs}
				/>
				<circle
					cx={0}
					cy={0}
					r={getBranchNodeRadius(data)}
					style={strokeStyles}
				/>
				<JotaiCircle
					cx={0}
					cy={0}
					r={getBranchNodeRadius(data)}
					style={focusStyles}
				/>
			</g>
		</g>
	);
}
