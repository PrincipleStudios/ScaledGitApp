import { useTranslation } from 'react-i18next';
import { Container } from '@/components/common';

export function BranchConflictsComponent({ name }: { name: string[] }) {
	return <BranchConflictsComponentPresentation branchNames={name} />;
}

export function BranchConflictsComponentPresentation({
	branchNames,
}: {
	branchNames: string[];
}) {
	const { t } = useTranslation('branch-conflicts');
	return (
		<Container.Responsive>
			{t('TODO', { replace: { branches: branchNames.join(' | ') } })}
		</Container.Responsive>
	);
}
