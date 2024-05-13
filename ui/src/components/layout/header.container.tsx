import { useCallback } from 'react';
import { useTranslation } from 'react-i18next';
import { useMutation } from '@tanstack/react-query';
import { queries } from '@/utils/api/queries';
import { useLaunchModal } from '@/utils/modal';
import { HeaderPresentation } from './header.presentation';
import type { HeaderPresentationalProps } from './header.presentation';
import { SearchDialog } from './SearchDialog';

export function useHeader(): React.ComponentType<HeaderPresentationalProps> {
	return useCallback(function HeaderContainer(
		props: HeaderPresentationalProps,
	) {
		const { t } = useTranslation(['app']);
		const launchModal = useLaunchModal();
		const fetch = useMutation(queries.requestGitFetch);
		return (
			<HeaderPresentation
				{...props}
				onSearch={handleSearch}
				onRefresh={() => !fetch.isPending && fetch.mutate()}
				isRefreshing={fetch.isPending}
			/>
		);

		function handleSearch() {
			void launchModal({
				ModalContents: SearchDialog,
				label: t('search-for-branches'),
			});
		}
	}, []);
}
