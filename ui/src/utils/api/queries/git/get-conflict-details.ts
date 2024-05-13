import type { QueryOptions } from '@tanstack/react-query';
import { api } from '../../fetch-api';

export const getConflictDetails = (branchNames: string[]) =>
	({
		queryKey: ['conflict-details', branchNames],
		queryFn: async () => {
			const response = await api.getConflictDetails({
				body: {
					branches: branchNames,
				},
			});
			if (response.statusCode !== 200) return Promise.reject(response);
			return response.data;
		},
	}) satisfies QueryOptions;
