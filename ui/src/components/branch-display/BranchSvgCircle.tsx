import { useTranslation } from 'react-i18next';
import { TooltipLine } from '../common';
import { useTooltipReference } from '../tooltips';
import { isDetailed, type BranchInfo } from './types';

/** A node in a branch graph to represent a branch */
export function BranchSvgCircle({ data }: { data: BranchInfo }) {
	const tooltip = useTooltipReference(() => (
		<BranchNodeTooltip details={data} />
	));
	const details = isDetailed(data)
		? data
		: { ...data, nonMergeCommitCount: null, exists: null };
	const styles: React.CSSProperties = details.detailed
		? {
				fill: details.exists ? details.color : 'transparent',
				opacity: (details.nonMergeCommitCount ?? 1) > 0 ? 1 : 0.5,
				stroke: details.color,
			}
		: {
				fill: 'rgba(0.5,0.5,0.5,0.25)',
				opacity: 1,
				strokeDasharray: '3,3',
				stroke: 'black',
			};
	return (
		<g {...tooltip()}>
			<circle cx={0} cy={0} r={5} style={styles} />
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
