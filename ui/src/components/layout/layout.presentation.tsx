import { twMerge } from 'tailwind-merge';
import { elementTemplate } from '../templating';
import { Tooltips } from '../tooltips';
import type { HeaderPresentationalProps } from './header.presentation';
import styles from './layout.module.css';
import { LoadingSection } from './LoadingSection';

export type LayoutProps = {
	header: React.ComponentType<HeaderPresentationalProps>;
	children?: React.ReactNode;
};

const LayoutContainer = elementTemplate('LayoutContainer', 'div', (T) => (
	<T className={styles.layout} />
));

const base = elementTemplate('Base', 'section', (T) => (
	<T tabIndex={0} className="overflow-auto text-slate-950 dark:text-white" />
));
const Main = base.extend('Main', () => (
	<main
		tabIndex={-1}
		className={twMerge('bg-white dark:bg-slate-950', styles.main)}
	/>
));

export function LayoutPresentation({ header: Header, children }: LayoutProps) {
	return (
		<LayoutContainer>
			<LoadingSection>
				<Header />
				<Main>{children}</Main>
			</LoadingSection>
			<Tooltips />
		</LayoutContainer>
	);
}
