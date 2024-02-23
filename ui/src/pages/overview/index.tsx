import { useBranchGraph } from '../../components/branch-graph';
import { useGitVersion } from './useGitVersion';

export function OverviewComponent() {
	const GitVersion = useGitVersion();
	const BranchGraph = useBranchGraph();
	return (
		<>
			<GitVersion />
			<BranchGraph />
		</>
	);
}
