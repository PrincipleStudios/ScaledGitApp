import { useTranslation } from 'react-i18next';
import type { NavigateFunction } from 'react-router-dom';
import { useNavigate } from 'react-router-dom';
import { useSuspenseQueries } from '@tanstack/react-query';
import { BranchGraphPresentation } from '../../components/branch-graph/branch-graph.presentation';
import { Container } from '../../components/common';
import { Tab } from '../../components/tabs';
import { queries } from '../../utils/api/queries';
import styles from './branch-details.module.css';
import { DetailsPanel } from './DetailsPanel';
import { useBranchDetails } from './useBranchDetails';
import { namesOf } from './utils';
import type { BranchDetails } from '../../generated/api/models';

export function BranchDetailsComponent({ name }: { name: string[] }) {
	const navigate = useNavigate();
	const mainBranchDetails = useSuspenseQueries({
		queries: name.map(queries.getBranchDetails),
	}).map((result) => result.data);
	const allBranchDetails = useBranchDetails(name);
	return (
		<BranchDetailsComponentPresentation
			navigate={navigate}
			mainBranchDetails={mainBranchDetails}
			allBranchDetails={allBranchDetails}
		/>
	);
}

function BranchDetailsComponentPresentation({
	navigate,
	mainBranchDetails,
	allBranchDetails,
}: {
	navigate: NavigateFunction;
	mainBranchDetails: BranchDetails[];
	allBranchDetails: BranchDetails[];
}) {
	const { t } = useTranslation('branch-details');
	return (
		<Container.Responsive className={styles.container}>
			<Tab.Group>
				<Tab.List className={styles.tabList}>
					<Tab>{t('tabs.details')}</Tab>
					<Tab>{t('tabs.suggestions')}</Tab>
				</Tab.List>
				<BranchGraphPresentation
					upstreamData={allBranchDetails}
					onClick={(node) => navigate({ search: `?name=${node.name}` })}
				/>

				{/* Key forces remount of all panels when main branches change */}
				<Tab.Panels key={namesOf(mainBranchDetails).join(',')}>
					<Tab.Panel>
						<DetailsPanel branches={mainBranchDetails} />
					</Tab.Panel>
					<Tab.Panel>Two</Tab.Panel>
				</Tab.Panels>
			</Tab.Group>
		</Container.Responsive>
	);
}
