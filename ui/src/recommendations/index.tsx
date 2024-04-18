import { useMemo } from 'react';
import { currentValue } from '@principlestudios/jotai-utilities/currentValue';
import { useQueryClient } from '@tanstack/react-query';
import type { Atom } from 'jotai';
import { atom, useAtomValue } from 'jotai';
import { type Loadable } from 'jotai/vanilla/utils/loadable';
import type { BranchDetails } from '@/generated/api/models';
import { useSuspensePromise } from '@/utils/useSuspensePromise';
import { flattenAnalyzeResult } from './flattenAnalyzeResult';
import { loadAllRules } from './load-all-rules';
import type {
	LoadableRecommendations,
	RecommendationContext,
	RecommendationOutput,
} from './rule-base';

export type {
	RecommendationsEngine,
	Recommendation,
	LoadableRecommendations,
	RecommendationContext,
} from './rule-base';

type LoadableOutput = Loadable<RecommendationOutput>;
function isLoading(output: LoadableOutput): output is { state: 'loading' } {
	return 'state' in output && output.state === 'loading';
}
function currentData(output: LoadableOutput) {
	return 'state' in output
		? output.state === 'hasData'
			? output.data
			: []
		: output;
}

export function useRecommendations(branches: BranchDetails[]) {
	const queryClient = useQueryClient();
	const allRules = useSuspensePromise(loadAllRules());
	const resultAtom = useMemo(() => {
		const context: RecommendationContext = { queryClient };
		const analysis = allRules
			.map((rule) => rule.analyze(branches, context))
			.flatMap<LoadableOutput[] | Atom<LoadableOutput[]>>(flattenAnalyzeResult);
		return atom((get): LoadableRecommendations => {
			const currentResults = analysis.flatMap((v) => currentValue(v, get));
			return {
				state: currentResults.some(isLoading) ? 'loading' : 'hasData',
				data: currentResults
					.flatMap(currentData)
					.sort((a, b) => a.priority - b.priority),
			};
		});
	}, [allRules, branches, queryClient]);
	return useAtomValue(resultAtom);
}
