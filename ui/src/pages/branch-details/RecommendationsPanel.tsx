import { useCallback } from 'react';
import { useTranslation } from 'react-i18next';
import { Code } from '@/components/code/Code';
import { Section } from '@/components/common';
import { LoadingSection } from '@/components/layout/LoadingSection';
import { Heading, HintText } from '@/components/text';
import type { BranchDetails } from '@/generated/api/models';
import { useRecommendations } from '@/recommendations';
import type {
	Recommendation,
	LoadableRecommendations,
} from '@/recommendations';

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
			{recommendation.commands ? (
				<Code>
					{recommendation.commands.map((command) => `${command}\n`).join('')}
				</Code>
			) : null}
			{recommendation.seeAlso ? (
				<ul>
					{recommendation.seeAlso.map((s) => (
						<li key={s.key}>
							<a className="text-blue-800 underline" href={s.url}>
								{t(s.translationKey, { replace: s.translationParameters })}
							</a>
						</li>
					))}
				</ul>
			) : null}
		</Section.SingleColumn>
	);
}
