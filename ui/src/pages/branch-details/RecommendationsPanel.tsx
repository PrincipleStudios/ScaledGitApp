import { useCallback } from 'react';
import { useTranslation } from 'react-i18next';
import { useRecommendationsEngine } from '../../recommendations';
import type { BranchDetails } from '../../generated/api/models';
import type { RecommendationsEngine } from '../../recommendations';

export type RecommendationsPanelComponent = React.FC<{
	branches: BranchDetails[];
}>;

export function useRecommendationsPanel(): RecommendationsPanelComponent {
	return useCallback(function RecommendationsPanelContainer({ branches }) {
		const engine = useRecommendationsEngine();
		return (
			<RecommendationsPanel
				branches={branches}
				recommendationsEngine={engine}
			/>
		);
	}, []);
}

export function RecommendationsPanel({
	branches,
	recommendationsEngine,
}: {
	branches: BranchDetails[];
	recommendationsEngine: RecommendationsEngine;
}) {
	const { t } = useTranslation('branch-details', {
		keyPrefix: 'recommendations',
	});
	const recommendations = recommendationsEngine.getRecommendations(branches);
	if (recommendations.length === 0) return t('none');

	return <ul>TODO</ul>;
}
