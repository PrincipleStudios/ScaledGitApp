import type { BranchDetails } from '../generated/api/models';

export type RecommendationsEngine = {
	getRecommendations: (branches: BranchDetails[]) => Recommendation[];
};

export type Recommendation = {
	description: string;
	commands: string[];
};

export function useRecommendationsEngine(): RecommendationsEngine {
	return {
		getRecommendations(branches) {
			return [];
		},
	};
}
