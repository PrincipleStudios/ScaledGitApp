import { useCallback } from 'react';
import { useTranslation } from 'react-i18next';
import {
	useBranchActions,
	useBranchActionTranslation,
	type BranchAction,
	type LoadableBranchActions,
} from '@/branch-actions';
import type { ActionComponentProps } from '@/branch-actions/branch-action-base';
import { Section } from '@/components/common';
import { LoadingSection } from '@/components/layout/LoadingSection';
import { Heading, HintText } from '@/components/text';
import type { BranchDetails } from '@/generated/api/models';

export type BranchActionsPanelComponent = React.FC<{
	branches: BranchDetails[];
}>;

export function useBranchActionsPanel(): BranchActionsPanelComponent {
	return useCallback(function BranchActionsPanelContainer({ branches }) {
		const branchActions = useBranchActions(branches);
		return (
			<BranchActionsPanel branchActions={branchActions} branches={branches} />
		);
	}, []);
}

export function BranchActionsPanel({
	branchActions: { state, data: branchActions },
	...rest
}: {
	branchActions: LoadableBranchActions;
} & ActionComponentProps) {
	const { t } = useTranslation('branchActions');
	if (branchActions.length === 0) {
		if (state === 'loading') return <LoadingSection />;
		return t('none');
	}

	return (
		<ul>
			{state === 'loading' ? (
				<li>
					<LoadingSection />
				</li>
			) : null}
			{branchActions.map((branchAction, index) => (
				<li key={index}>
					<BranchActionPresentation branchAction={branchAction} {...rest} />
				</li>
			))}
		</ul>
	);
}

function BranchActionPresentation({
	branchAction: { ActionComponent, translationKey, translationParameters },
	...rest
}: {
	branchAction: BranchAction;
} & ActionComponentProps) {
	const { t } = useBranchActionTranslation(translationKey);
	const opts = { replace: translationParameters };

	return (
		<Section.SingleColumn>
			<Heading.Section>{t('title', opts)}</Heading.Section>
			<HintText>{t('description', opts)}</HintText>
			<ActionComponent {...rest} />
		</Section.SingleColumn>
	);
}
