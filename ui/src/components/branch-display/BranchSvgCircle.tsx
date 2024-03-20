import {
	useComputedAtom,
	type CSSPropertiesWithSignal,
} from '@principlestudios/jotai-react-signals';
import { JotaiCircle } from '../svg/atom-elements';
import { useTooltipReference } from '../tooltips';
import { activeBranchNames } from './active';
import { BranchNodeTooltip } from './BranchNodeTooltip';
import { isDetailed, type BranchInfo } from './types';
import { useActiveBranchOnHover } from './useActiveBranchOnHover';

export const branchNodeRadius = 5;
export const branchNodeStrokeWidth = 1;
export const branchNodeRadiusWithStroke =
	branchNodeRadius + branchNodeStrokeWidth / 2;

/** A node in a branch graph to represent a branch. Centers branch at 0,0 - use
 * transforms outside of this item to position elsewhere. */
export function BranchSvgCircle({ data }: { data: BranchInfo }) {
	const tooltip = useTooltipReference(() => (
		<BranchNodeTooltip details={data} />
	));
	const details = isDetailed(data)
		? data
		: { ...data, nonMergeCommitCount: null, exists: null };
	const outerStyles: React.CSSProperties = {
		color: details.color ?? 'rgba(128, 128, 128)',
	};
	const strokeStyles: React.CSSProperties = {
		fill: 'none',
		strokeDasharray: details.color ? undefined : '3,3',
		stroke: 'currentcolor',
		strokeWidth: branchNodeStrokeWidth,
	};
	const fillStyles: React.CSSProperties = {
		...strokeStyles,
		fill: isDetailed(data) && !data.exists ? 'transparent' : 'currentcolor',
		fillOpacity: details.color ? 1 : 0.5,
		opacity: (details.nonMergeCommitCount ?? 1) > 0 ? 1 : 0.5,
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
	return (
		<g {...tooltip()}>
			<g {...onHover} style={outerStyles}>
				<circle cx={0} cy={0} r={branchNodeRadius} style={fillStyles} />
				<JotaiCircle cx={0} cy={0} r={branchNodeRadius} style={focusStyles} />
			</g>
		</g>
	);
}
