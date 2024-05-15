import type { QueryOptions } from '@tanstack/react-query';
import { api } from '../../fetch-api';

export const retrieveGitObject = (objectish: string) =>
	({
		queryKey: ['git-object', objectish],
		queryFn: async () => {
			const response = await api.retrieveGitObject({
				params: { objectish },
			});
			if (response.statusCode !== 200 && response.statusCode !== 206)
				return Promise.reject(response);
			return { status: response.statusCode, data: response.data };
		},
	}) satisfies QueryOptions;
