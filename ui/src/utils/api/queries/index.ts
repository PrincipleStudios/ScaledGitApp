import { getLoginSchemes } from './auth/get-login-schemes';
import { getInfo } from './environment';
import { getBranchDetails } from './git/branch-details';
import { requestGitFetch } from './git/fetch';
import { getUpstreamData } from './git/upstream-data';
import type operations from '../../../generated/api/operations';

export const queries = {
	getInfo,
	getLoginSchemes,
	/** Intentionally not supporting `getTranslationData` here */
	getTranslationData: null,

	requestGitFetch,
	getUpstreamData,
	getBranchDetails,
} satisfies { [K in keyof typeof operations]: unknown };
