import { useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import {
	useComputedAtom,
	type CSSPropertiesWithSignal,
} from '@principlestudios/jotai-react-signals';
import { useSetAtom } from 'jotai';
import { TooltipLine } from '../common';
import { JotaiCircle } from '../svg/atom-elements';
import { useTooltipReference } from '../tooltips';
import { activeBranchNames } from './active';
import { isDetailed, type BranchInfo } from './types';

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
	const setActiveBranches = useSetAtom(activeBranchNames);
	useEffect(function onUnmountRemoveFromActiveBranches() {
		return () => setActiveBranches({ remove: data.name });
	});
	return (
		<g {...tooltip()}>
			<g
				onMouseEnter={() => setActiveBranches({ add: data.name })}
				onMouseLeave={() => setActiveBranches({ remove: data.name })}
				style={outerStyles}
			>
				<circle cx={0} cy={0} r={branchNodeRadius} style={fillStyles} />
				<JotaiCircle cx={0} cy={0} r={branchNodeRadius} style={focusStyles} />
			</g>
		</g>
	);
}

function BranchNodeTooltip({ details }: { details: BranchInfo }) {
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
