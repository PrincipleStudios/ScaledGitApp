import { useMemo } from 'react';
import { currentValue } from '@principlestudios/jotai-utilities/currentValue';
import { isAtom } from '@principlestudios/jotai-utilities/isAtom';
import { useQueryClient } from '@tanstack/react-query';
import { atom, useAtomValue } from 'jotai';
import { loadable } from 'jotai/utils';
import { type Loadable } from 'jotai/vanilla/utils/loadable';
import type { BranchDetails } from '@/generated/api/models';
import { useSuspensePromise } from '@/utils/useSuspensePromise';
import { loadAllRules } from './load-all-rules';
import type { RecommendationOutput, RecommendationsEngine } from './rule-base';

export type {
	RecommendationsEngine,
	Recommendation,
	LoadableRecommendations,
	RecommendationContext,
} from './rule-base';

type Output = RecommendationOutput[] | Loadable<RecommendationOutput[]>;
function isLoading(output: Output): output is { state: 'loading' } {
	return !Array.isArray(output) && output.state === 'loading';
}
function currentData(output: Output) {
	return Array.isArray(output)
		? output
		: output.state === 'hasData'
			? output.data
			: [];
}

export function useRecommendationsEngine(): RecommendationsEngine {
	const allRules = useSuspensePromise(loadAllRules());

	return useMemo(
		() => ({
			getRecommendations(branches, context) {
				const analysis = allRules
					.map((rule) => rule.analyze(branches, context))
					.map((output) => (isAtom(output) ? loadable(output) : output));
				return atom((get) => {
					const currentResults = analysis.map((v) =>
						currentValue<Output>(v, get),
					);
					return {
						state: currentResults.some(isLoading) ? 'loading' : 'hasData',
						data: currentResults
							.flatMap(currentData)
							.sort((a, b) => a.priority - b.priority),
					};
				});
			},
		}),
		[allRules],
	);
}

export function useRecommendations(branches: BranchDetails[]) {
	const queryClient = useQueryClient();
	const engine = useRecommendationsEngine();
	const resultAtom = useMemo(
		() =>
			engine.getRecommendations(branches, {
				queryClient,
			}),
		[engine, branches, queryClient],
	);
	return useAtomValue(resultAtom);
}
