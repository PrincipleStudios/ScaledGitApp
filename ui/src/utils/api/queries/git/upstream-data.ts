import { api } from '../../fetch-api';

export const getUpstreamData = {
	queryKey: ['upstream-data'],
	queryFn: async () => {
		const response = await api.getUpstreamData();
		if (response.statusCode !== 200) return Promise.reject(response);
		return response;
	},
};
