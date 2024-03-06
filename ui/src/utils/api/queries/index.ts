import { getInfo } from './environment';
import { requestGitFetch } from './git/fetch';
import { getUpstreamData } from './git/upstream-data';
import type operations from '../../../generated/api/operations';

export const queries = {
	getInfo,
	/** Intentionally not supporting `getTranslationData` here */
	getTranslationData: null,

	requestGitFetch,
	getUpstreamData,
	// TODO: add with UI visualization
	getBranchDetails: null,
} satisfies { [K in keyof typeof operations]: unknown };
