import { useTranslation } from 'react-i18next';
import { TooltipLine } from '../common';
import { useTooltipReference } from '../tooltips';
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
	const styles: React.CSSProperties = {
		fill:
			isDetailed(data) && !data.exists
				? 'transparent'
				: details.color ?? 'rgba(128, 128, 128, 0.5)',
		opacity: (details.nonMergeCommitCount ?? 1) > 0 ? 1 : 0.5,
		strokeDasharray: details.color ? undefined : '3,3',
		stroke: details.color ?? 'rgb(128,128,128)',
		strokeWidth: branchNodeStrokeWidth,
	};
	return (
		<g {...tooltip()}>
			<circle cx={0} cy={0} r={branchNodeRadius} style={styles} />
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
