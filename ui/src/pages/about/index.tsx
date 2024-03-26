import { Section } from '@/components/common';
import { useGitVersion } from './useGitVersion';

export function AboutComponent() {
	const GitVersion = useGitVersion();
	return (
		<Section.SingleColumn>
			<GitVersion />
		</Section.SingleColumn>
	);
}
