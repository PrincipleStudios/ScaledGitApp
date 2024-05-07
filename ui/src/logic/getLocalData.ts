import type {
	DefaultError,
	FetchQueryOptions,
	QueryClient,
	QueryKey,
} from '@tanstack/react-query';

export function getLocalData<
	TQueryFnData,
	TError = DefaultError,
	TData = TQueryFnData,
	TQueryKey extends QueryKey = QueryKey,
>(
	queryClient: QueryClient,
	options: FetchQueryOptions<TQueryFnData, TError, TData, TQueryKey, never>,
): TData | undefined {
	return queryClient.getQueryData<TQueryFnData, TQueryKey, TData>(
		options.queryKey,
	);
}
