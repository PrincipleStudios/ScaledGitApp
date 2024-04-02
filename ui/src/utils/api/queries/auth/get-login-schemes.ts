import { api } from '../../fetch-api';

export const getLoginSchemes = {
	queryKey: ['env'],
	queryFn: async () => {
		const response = await api.getLoginSchemes();
		if (response.statusCode !== 200) return Promise.reject(response);
		return response.data;
	},
};
