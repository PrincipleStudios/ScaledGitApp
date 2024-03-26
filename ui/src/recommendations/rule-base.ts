import type { BranchDetails } from '@/generated/api/models';

export type RecommendationsEngine = {
	getRecommendations: (branches: BranchDetails[]) => Recommendation[];
};

export type Recommendation = {
	translationKey: string;
	commands: string[];
	translationParameters?: Record<string, string | number>;
};

export type RecommendationOutput = Recommendation & {
	/** A relatively magic number used to prioritize recommendations. Lower is higher priority. */
	priority: number;
};
export type RecommendationRule = {
	analyze(branches: BranchDetails[]): RecommendationOutput[];
};
