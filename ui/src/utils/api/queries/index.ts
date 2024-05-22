import type operations from '@/generated/api/operations';
import { getLoginSchemes } from './auth/get-login-schemes';
import { getInfo } from './environment';
import { getBranchDetails } from './git/branch-details';
import { requestGitFetch } from './git/fetch';
import { getConflictDetails } from './git/get-conflict-details';
import { retrieveGitObject } from './git/retrieve-git-object';
import { getUpstreamData } from './git/upstream-data';

export const queries = {
	getInfo,
	getLoginSchemes,
	/** Intentionally not supporting `getTranslationData` here */
	getTranslationData: null,

	requestGitFetch,
	getUpstreamData,
	getBranchDetails,

	getConflictDetails,
	retrieveGitObject,
} satisfies { [K in keyof typeof operations]: unknown };

console.log(queries);
