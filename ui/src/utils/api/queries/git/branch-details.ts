import { api } from '../../fetch-api';

export const getBranchDetails = (branchNames: string[]) => ({
	queryKey: ['branch-details', branchNames],
	queryFn: async () => {
		const response = await api.getBranchDetails({
			body: {
				branches: branchNames,
				includeDownstream: true,
				includeUpstream: true,
				recurse: true,
			},
		});
		if (response.statusCode !== 200) return Promise.reject(response);
		return response.data;
	},
});
