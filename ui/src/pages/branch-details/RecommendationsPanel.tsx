import { useCallback } from 'react';
import { useTranslation } from 'react-i18next';
import { Section } from '../../components/common';
import { Code, Heading, HintText } from '../../components/text';
import { useRecommendationsEngine } from '../../recommendations';
import type { BranchDetails } from '../../generated/api/models';
import type {
	RecommendationsEngine,
	Recommendation,
} from '../../recommendations';

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
	const { t } = useTranslation('recommendations');
	const recommendations = recommendationsEngine.getRecommendations(branches);
	if (recommendations.length === 0) return t('none');

	return (
		<ul>
			{recommendations.map((recommendation, index) => (
				<li key={index}>
					<RecommendationPresentation recommendation={recommendation} />
				</li>
			))}
		</ul>
	);
}

function RecommendationPresentation({
	recommendation,
}: {
	recommendation: Recommendation;
}) {
	const { t } = useTranslation('recommendations', {
		keyPrefix: recommendation.translationKey,
	});
	const opts = { replace: recommendation.translationParameters };

	return (
		<Section.SingleColumn>
			<Heading.Section>{t('title', opts)}</Heading.Section>
			<HintText>{t('description', opts)}</HintText>
			<Code>{recommendation.commands.join('\n')}</Code>
		</Section.SingleColumn>
	);
}
