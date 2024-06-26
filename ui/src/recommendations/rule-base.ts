import { type QueryClient } from '@tanstack/react-query';
import { type Atom } from 'jotai';
import type { Loadable } from 'jotai/vanilla/utils/loadable';
import { type BranchDetails } from '@/generated/api/models';

// Not using Jotai's Loadable<> here because it doesn't allow for partial data
export type LoadableRecommendations = {
	state: 'loading' | 'hasData';
	data: Recommendation[];
};
export type RecommendationsEngine = {
	getRecommendations: (
		branches: BranchDetails[],
		context: RecommendationContext,
	) => Atom<LoadableRecommendations>;
};

export type RecommendationContext = {
	/** Access to the query client for loading other branches, etc. */
	queryClient: QueryClient;
};

export type TranslationMeta = {
	translationKey: string;
	translationParameters?: Record<string, string | number>;
};

export type RecommendationSeeAlso = {
	key: string;
	url: string;
} & TranslationMeta;

export type Recommendation = {
	recommendationKey: string;
} & TranslationMeta &
	(
		| {
				commands?: undefined;
				seeAlso: RecommendationSeeAlso[];
		  }
		| {
				commands: string[];
				seeAlso?: undefined;
		  }
	);

export type RecommendationOutput = Recommendation & {
	/** A relatively magic number used to prioritize recommendations. Lower is higher priority. */
	priority: number;
};
export type RecommendationRuleAnalyzeResult =
	| Promise<RecommendationOutput[]>
	| Array<
			| RecommendationOutput
			| Promise<RecommendationOutput[]>
			| Atom<Loadable<RecommendationOutput[]>>
			| Atom<Array<Loadable<RecommendationOutput[]>>>
	  >;

export type RecommendationRule = {
	analyze(
		branches: BranchDetails[],
		context: RecommendationContext,
	): RecommendationRuleAnalyzeResult;
};
