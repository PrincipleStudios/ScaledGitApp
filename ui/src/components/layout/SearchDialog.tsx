import { Fragment } from 'react';
import { Trans, useTranslation } from 'react-i18next';
import { useLocation, type Location } from 'react-router-dom';
import without from 'lodash/without';
import { BulletList, Button, Link, Section } from '@/components/common';
import { ModalDialogLayout } from '@/components/modals/modal-dialog';
import type { BranchConfiguration } from '@/generated/api/models';
import type { ModalContentsProps } from '@/utils/modal/modal-service';
import { parseSearchString, toSearchString } from '@/utils/search-string';
import { BranchName } from '../branch-display/BranchName';
import type { BranchListingProps } from '../branch-listing';
import { useBranchListing } from '../branch-listing';

export function SearchDialog({ resolve }: ModalContentsProps<void, never>) {
	const BranchListing = useBranchListing();
	const location = useLocation();

	return (
		<SearchDialogPresentation
			BranchListing={BranchListing}
			location={location}
			onClose={() => resolve()}
		/>
	);
}

function SearchDialogPresentation({
	location,
	BranchListing,
	onClose,
}: {
	location: Location;
	BranchListing: React.ComponentType<BranchListingProps>;
	onClose: () => void;
}) {
	const { t } = useTranslation(['app'], { keyPrefix: 'search-dialog' });
	const queryString = parseSearchString(location.search);

	return (
		<ModalDialogLayout
			buttons={<Button onClick={onClose}>{t('ok')}</Button>}
			title={t('title')}
		>
			<BranchListing>{listBranches}</BranchListing>
		</ModalDialogLayout>
	);

	function listBranches(branches: BranchConfiguration[]) {
		return (
			<Section.SingleColumn>
				<BulletList className="h-[60vh] md:h-80 overflow-auto">
					{branches.map((branch) => {
						const extraLink = extraLinkFor(branch);
						return (
							<Fragment key={branch.name}>
								<BulletList.Item>
									<Link
										to={`/branch?name=${encodeURIComponent(branch.name)}`}
										onClick={onClose}
									>
										<BranchName data={branch} />
									</Link>{' '}
								</BulletList.Item>
								{extraLink && (
									<BulletList.Item className="ml-8">
										{extraLink}
									</BulletList.Item>
								)}
							</Fragment>
						);
					})}
				</BulletList>
			</Section.SingleColumn>
		);
	}

	function extraLinkFor(branch: BranchConfiguration) {
		if (!queryString['name']) return null;
		const removing = queryString['name'].includes(branch.name);
		const name = removing
			? without(queryString['name'], branch.name)
			: [...queryString['name'], branch.name];
		const i18nKey = removing ? 'remove' : 'add';
		return (
			<Link
				to={{
					...location,
					search: toSearchString({ ...queryString, name }),
				}}
				onClick={onClose}
			>
				<Trans
					t={t}
					i18nKey={i18nKey}
					components={{ Branch: <>{branch.name}</> }}
				/>
			</Link>
		);
	}
}
