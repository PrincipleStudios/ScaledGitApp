import { api } from '../../fetch-api';

export const requestGitFetch = {
	mutationFn: async () => {
		const response = await api.requestGitFetch();
		if (response.statusCode !== 200) return Promise.reject(response);
	},
};
