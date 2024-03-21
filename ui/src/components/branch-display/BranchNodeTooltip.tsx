import { useTranslation } from 'react-i18next';
import { TooltipLine } from '../common';
import { isDetailed, type BranchInfo } from './types';

export function BranchNodeTooltip({ details }: { details: BranchInfo }) {
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
