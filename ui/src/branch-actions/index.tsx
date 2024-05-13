import { useMemo } from 'react';
import { currentValue } from '@principlestudios/jotai-utilities/currentValue';
import { useQueryClient } from '@tanstack/react-query';
import type { Atom } from 'jotai';
import { atom, useAtomValue } from 'jotai';
import { loadable } from 'jotai/utils';
import { type Loadable } from 'jotai/vanilla/utils/loadable';
import type { BranchDetails } from '@/generated/api/models';
import { useSuspensePromise } from '@/utils/useSuspensePromise';
import type {
	LoadableBranchActions,
	BranchActionProviderContext,
	BranchActionOutput,
} from './branch-action-base';
import { loadAllBranchActions } from './load-all-branch-actions';

export { useBranchActionTranslation } from './use-branch-action-translation';
export type {
	BranchAction,
	LoadableBranchActions,
	BranchActionProviderContext,
} from './branch-action-base';

type LoadableOutput = Loadable<null | BranchActionOutput>;
function isLoading(output: LoadableOutput): output is { state: 'loading' } {
	return 'state' in output && output.state === 'loading';
}
function currentData(output: LoadableOutput) {
	return 'state' in output
		? output.state === 'hasData'
			? output.data
			: null
		: output;
}

export function useBranchActions(branches: BranchDetails[]) {
	const queryClient = useQueryClient();
	const allProviders = useSuspensePromise(loadAllBranchActions());
	const resultAtom = useMemo(() => {
		const context: BranchActionProviderContext = { branches, queryClient };
		const analysis = allProviders
			.map((provider) => provider.provide(context))
			.flatMap<
				Atom<LoadableOutput>
			>((actionPromise) => loadable(atom(actionPromise)));
		return atom((get): LoadableBranchActions => {
			const currentResults = analysis.flatMap((v) => currentValue(v, get));
			return {
				state: currentResults.some(isLoading) ? 'loading' : 'hasData',
				data: currentResults
					.map(currentData)
					.filter((d): d is BranchActionOutput => !!d)
					.sort((a, b) => a.order - b.order),
			};
		});
	}, [allProviders, branches, queryClient]);
	return useAtomValue(resultAtom);
}
