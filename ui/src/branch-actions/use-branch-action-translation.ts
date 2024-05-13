import { useTranslation } from 'react-i18next';

export function useBranchActionTranslation(branchActionTranslationKey: string) {
	return useTranslation('actions', { keyPrefix: branchActionTranslationKey });
}
