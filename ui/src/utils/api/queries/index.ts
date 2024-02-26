import { getInfo } from './environment';
import type operations from '../../../generated/api/operations';

export const queries = {
	getInfo,
	/** Intentionally not supporting `getTranslationData` here */
	getTranslationData: null,
	/** TODO */
	requestGitFetch: null,
	/** TODO */
	getUpstreamData: null,
} satisfies { [K in keyof typeof operations]: unknown };
