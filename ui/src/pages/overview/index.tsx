import { useGitVersion } from './useGitVersion';

export function OverviewComponent() {
	const GitVersion = useGitVersion();
	return (
		<>
			<GitVersion />
		</>
	);
}
