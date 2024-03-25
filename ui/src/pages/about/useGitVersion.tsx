import { useCallback } from 'react';
import { queries } from '@/utils/api/queries';
import { useSuspenseQuery } from '@tanstack/react-query';

export function useGitVersion() {
	const result = useSuspenseQuery(queries.getInfo);
	return useCallback(
		() => (
			<>
				Hash: {result.data.gitHash}, Tag: {result.data.tag}
			</>
		),
		[result],
	);
}
