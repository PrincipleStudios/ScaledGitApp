import { type QueryClient } from '@tanstack/react-query';
import { type BranchDetails } from '@/generated/api/models';

// Not using Jotai's Loadable<> here because it doesn't allow for partial data
export type LoadableBranchActions = {
	state: 'loading' | 'hasData';
	data: BranchAction[];
};

export type BranchActionProviderContext = {
	branches: BranchDetails[];
	/** Access to the query client for loading other branches, etc. */
	queryClient: QueryClient;
};

export type ActionComponentProps = {
	branches: BranchDetails[];
};

export type TranslationMeta = {
	translationKey: string;
	translationParameters?: Record<string, string | number>;
};

export type BranchAction = {
	actionKey: string;
	ActionComponent: React.ComponentType<ActionComponentProps>;
} & TranslationMeta;

export type BranchActionOutput = BranchAction & {
	/** A relatively magic number used to order branch action. Lower numbers will be listed first. */
	order: number;
};

export type BranchActionProvider = {
	provide(
		context: BranchActionProviderContext,
	): null | BranchActionOutput | Promise<null | BranchActionOutput>;
};
