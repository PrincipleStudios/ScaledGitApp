import type { QueryOptions } from '@tanstack/react-query';
import { api } from '../../fetch-api';
import { responseToString } from '../../response-to-string';

export const retrieveGitObject = (objectish: string) =>
	({
		queryKey: ['git-object', objectish],
		queryFn: async () => {
			const response = await api.retrieveGitObject({
				params: { objectish },
			});
			if (response.response.status !== 200 && response.response.status !== 206)
				return Promise.reject(response);

			return {
				status: response.response.status,
				data: await responseToString(response.response),
			};
		},
	}) satisfies QueryOptions;
