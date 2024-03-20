import { useCallback } from 'react';
import { useTranslation } from 'react-i18next';
import { Section } from '../../components/common';
import { LoadingSection } from '../../components/layout/LoadingSection';
import { Code, Heading, HintText } from '../../components/text';
import { useRecommendations } from '../../recommendations';
import type { BranchDetails } from '../../generated/api/models';
import type {
	Recommendation,
	LoadableRecommendations,
} from '../../recommendations';

export type RecommendationsPanelComponent = React.FC<{
	branches: BranchDetails[];
}>;

export function useRecommendationsPanel(): RecommendationsPanelComponent {
	return useCallback(function RecommendationsPanelContainer({ branches }) {
		const recommendations = useRecommendations(branches);
		return <RecommendationsPanel recommendations={recommendations} />;
	}, []);
}

export function RecommendationsPanel({
	recommendations: { state, data: recommendations },
}: {
	recommendations: LoadableRecommendations;
}) {
	const { t } = useTranslation('recommendations');
	if (recommendations.length === 0) {
		if (state === 'loading') return <LoadingSection />;
		return t('none');
	}

	return (
		<ul>
			{state === 'loading' ? (
				<li>
					<LoadingSection />
				</li>
			) : null}
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
