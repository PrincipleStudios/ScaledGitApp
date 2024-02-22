import { api } from '../fetch-api';

export const getInfo = {
	queryKey: ['env'],
	queryFn: async () => {
		const response = await api.getInfo();
		if (response.statusCode !== 200) return Promise.reject(response);
		return response.data;
	},
};
