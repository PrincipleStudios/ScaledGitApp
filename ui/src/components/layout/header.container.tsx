import { useCallback } from 'react';
import { useMutation } from '@tanstack/react-query';
import { queries } from '../../utils/api/queries';
import { HeaderPresentation } from './header.presentation';
import type { HeaderPresentationalProps } from './header.presentation';

export function useHeader(): React.ComponentType<HeaderPresentationalProps> {
	return useCallback(function HeaderContainer(
		props: HeaderPresentationalProps,
	) {
		const fetch = useMutation(queries.requestGitFetch);
		return (
			<HeaderPresentation
				{...props}
				onRefresh={() => !fetch.isPending && fetch.mutate()}
				isRefreshing={fetch.isPending}
			/>
		);
	}, []);
}
