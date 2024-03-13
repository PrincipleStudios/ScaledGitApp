import { useNavigate } from 'react-router-dom';
import { BranchGraphPresentation } from '../../components/branch-graph/branch-graph.presentation';
import { Container } from '../../components/common';
import { Tab } from '../../components/tabs';
import styles from './branch-details.module.css';
import { useBranchDetails } from './useBranchDetails';

export function BranchDetailsComponent({ name }: { name: string[] }) {
	const navigate = useNavigate();
	const response = useBranchDetails(name);
	return (
		<Container.Responsive className={styles.container}>
			<Tab.Group>
				<Tab.List className={styles.tabList}>
					<Tab>One</Tab>
					<Tab>Two</Tab>
				</Tab.List>
				<BranchGraphPresentation
					upstreamData={response}
					onClick={(node) => navigate({ search: `?name=${node.name}` })}
				/>

				<Tab.Panels>
					<Tab.Panel>One</Tab.Panel>
					<Tab.Panel>Two</Tab.Panel>
				</Tab.Panels>
			</Tab.Group>
		</Container.Responsive>
	);
}
