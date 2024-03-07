import { useCallback } from 'react';
import { useSuspenseQuery } from '@tanstack/react-query';
import { queries } from '../../utils/api/queries';

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
