import { useTranslation } from 'react-i18next';
import type { NavigateFunction } from 'react-router-dom';
import { useNavigate } from 'react-router-dom';
import { BranchGraphPresentation } from '@/components/branch-graph/branch-graph.presentation';
import { Container } from '@/components/common';
import { LoadingSection } from '@/components/layout/LoadingSection';
import { Tab } from '@/components/tabs';
import type { BranchDetails } from '@/generated/api/models';
import { queries } from '@/utils/api/queries';
import { useSuspenseQueries } from '@tanstack/react-query';
import styles from './branch-details.module.css';
import { DetailsPanel } from './DetailsPanel';
import { useRecommendationsPanel } from './RecommendationsPanel';
import { useBranchDetails } from './useBranchDetails';
import { namesOf } from './utils';
import type { RecommendationsPanelComponent } from './RecommendationsPanel';

export function BranchDetailsComponent({ name }: { name: string[] }) {
	const navigate = useNavigate();
	const mainBranchDetails = useSuspenseQueries({
		queries: name.map(queries.getBranchDetails),
	}).map((result) => result.data);
	const allBranchDetails = useBranchDetails(name);
	const RecommendationsPanel = useRecommendationsPanel();
	return (
		<BranchDetailsComponentPresentation
			navigate={navigate}
			mainBranchDetails={mainBranchDetails}
			allBranchDetails={allBranchDetails}
			RecommendationsPanel={RecommendationsPanel}
		/>
	);
}

function BranchDetailsComponentPresentation({
	navigate,
	mainBranchDetails,
	allBranchDetails,
	RecommendationsPanel,
}: {
	navigate: NavigateFunction;
	mainBranchDetails: BranchDetails[];
	allBranchDetails: BranchDetails[];
	RecommendationsPanel: RecommendationsPanelComponent;
}) {
	const { t } = useTranslation('branch-details');
	return (
		<Container.Responsive className={styles.container}>
			<Tab.Group>
				<Tab.List className={styles.tabList}>
					<Tab>{t('tabs.details')}</Tab>
					<Tab>{t('tabs.recommendations')}</Tab>
				</Tab.List>
				<BranchGraphPresentation
					upstreamData={allBranchDetails}
					onClick={(node) => navigate({ search: `?name=${node.name}` })}
				/>

				{/* Key forces remount of all panels when main branches change */}
				<LoadingSection>
					<Tab.Panels key={namesOf(mainBranchDetails).join(',')}>
						<Tab.Panel>
							<DetailsPanel branches={mainBranchDetails} />
						</Tab.Panel>
						<Tab.Panel>
							<RecommendationsPanel branches={mainBranchDetails} />
						</Tab.Panel>
					</Tab.Panels>
				</LoadingSection>
			</Tab.Group>
		</Container.Responsive>
	);
}
