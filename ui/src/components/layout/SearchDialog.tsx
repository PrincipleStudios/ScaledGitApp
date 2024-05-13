import { useTranslation } from 'react-i18next';
import { BulletList, Button, Link, Section } from '@/components/common';
import { ModalDialogLayout } from '@/components/modals/modal-dialog';
import type { ModalContentsProps } from '@/utils/modal/modal-service';
import { BranchName } from '../branch-display/BranchName';
import { useBranchListing } from '../branch-listing';

export function SearchDialog({ resolve }: ModalContentsProps<void, never>) {
	const { t } = useTranslation(['app'], { keyPrefix: 'search-dialog' });
	const BranchListing = useBranchListing();

	return (
		<ModalDialogLayout
			buttons={
				<>
					<Button onClick={() => resolve()}>{t('ok')}</Button>
				</>
			}
			title={t('title')}
		>
			<BranchListing>
				{(branches) => (
					<Section.SingleColumn>
						<BulletList className="h-[60vh] md:h-80 overflow-auto">
							{branches.map((branch) => (
								<BulletList.Item key={branch.name}>
									<Link
										to={`/branch?name=${branch.name}`}
										onClick={() => resolve()}
									>
										<BranchName data={branch} />
									</Link>
								</BulletList.Item>
							))}
						</BulletList>
					</Section.SingleColumn>
				)}
			</BranchListing>
		</ModalDialogLayout>
	);
}
