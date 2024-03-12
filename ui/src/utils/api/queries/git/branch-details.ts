import { api } from '../../fetch-api';

export const getBranchDetails = (branchName: string) => ({
	queryKey: ['branch-details', branchName],
	queryFn: async () => {
		const response = await api.getBranchDetails({
			body: {
				branches: [branchName],
				includeDownstream: true,
				includeUpstream: true,
				recurse: true,
			},
		});
		if (response.statusCode !== 200) return Promise.reject(response);
		return response.data;
	},
});
