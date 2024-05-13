import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router-dom';
import { useSuspenseQueries } from '@tanstack/react-query';
import { BranchGraphPresentation } from '@/components/branch-graph/branch-graph.presentation';
import { Container } from '@/components/common';
import { LoadingSection } from '@/components/layout/LoadingSection';
import { Tab } from '@/components/tabs';
import type { Branch, BranchDetails } from '@/generated/api/models';
import { queries } from '@/utils/api/queries';
import styles from './branch-details.module.css';
import type { BranchActionsPanelComponent } from './BranchActionsPanel';
import { useBranchActionsPanel } from './BranchActionsPanel';
import { DetailsPanel } from './DetailsPanel';
import { useRecommendationsPanel } from './RecommendationsPanel';
import type { RecommendationsPanelComponent } from './RecommendationsPanel';
import { useBranchDetails } from './useBranchDetails';
import { namesOf } from './utils';

export function BranchDetailsComponent({ name }: { name: string[] }) {
	const navigate = useNavigate();
	const mainBranchDetails = useSuspenseQueries({
		queries: name.map(queries.getBranchDetails),
	}).map((result) => result.data);
	const allBranchDetails = useBranchDetails(name);
	const RecommendationsPanel = useRecommendationsPanel();
	const BranchActionsPanel = useBranchActionsPanel();
	return (
		<BranchDetailsComponentPresentation
			mainBranchDetails={mainBranchDetails}
			allBranchDetails={allBranchDetails}
			RecommendationsPanel={RecommendationsPanel}
			BranchActionsPanel={BranchActionsPanel}
			onClickNode={onClickNode}
		/>
	);

	function onClickNode(node: Branch, ev: MouseEvent) {
		if (ev.ctrlKey || ev.shiftKey) {
			navigate({
				search: `?name=${[...name, node.name].map(encodeURIComponent).join('&name=')}`,
			});
		} else {
			navigate({ search: `?name=${encodeURIComponent(node.name)}` });
		}
	}
}

function BranchDetailsComponentPresentation({
	mainBranchDetails,
	allBranchDetails,
	RecommendationsPanel,
	BranchActionsPanel,
	onClickNode,
}: {
	mainBranchDetails: BranchDetails[];
	allBranchDetails: BranchDetails[];
	RecommendationsPanel: RecommendationsPanelComponent;
	BranchActionsPanel: BranchActionsPanelComponent;
	onClickNode?: (node: Branch, ev: MouseEvent) => void;
}) {
	const { t } = useTranslation('branch-details');
	return (
		<Container.Responsive className={styles.container}>
			<Tab.Group>
				<Tab.List className={styles.tabList}>
					<Tab>{t('tabs.details')}</Tab>
					<Tab>{t('tabs.recommendations')}</Tab>
					<Tab>{t('tabs.branch-actions')}</Tab>
				</Tab.List>
				<BranchGraphPresentation
					upstreamData={allBranchDetails}
					onClick={onClickNode}
				/>

				{/* Key forces remount of all panels when main branches change */}
				<Tab.Panels key={namesOf(mainBranchDetails).join(',')}>
					<Tab.Panel>
						<LoadingSection>
							<DetailsPanel branches={mainBranchDetails} />
						</LoadingSection>
					</Tab.Panel>
					<Tab.Panel>
						<LoadingSection>
							<RecommendationsPanel branches={mainBranchDetails} />
						</LoadingSection>
					</Tab.Panel>
					<Tab.Panel>
						<LoadingSection>
							<BranchActionsPanel branches={mainBranchDetails} />
						</LoadingSection>
					</Tab.Panel>
				</Tab.Panels>
			</Tab.Group>
		</Container.Responsive>
	);
}
