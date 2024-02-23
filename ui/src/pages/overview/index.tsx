import { useBranchGraph } from '../../components/branch-graph';
import styles from './overview.module.css';
import { useGitVersion } from './useGitVersion';

export function OverviewComponent() {
	const GitVersion = useGitVersion();
	const BranchGraph = useBranchGraph();
	return (
		<div className={styles.container}>
			<GitVersion />
			<BranchGraph />
		</div>
	);
}
