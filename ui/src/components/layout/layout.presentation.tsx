import { twMerge } from 'tailwind-merge';
import { elementTemplate } from '../templating';
import styles from './layout.module.css';
import { LoadingSection } from './LoadingSection';

export type LayoutProps = {
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

export function LayoutPresentation({ children }: LayoutProps) {
	return (
		<LayoutContainer>
			<LoadingSection>
				<Main>{children}</Main>
			</LoadingSection>
		</LayoutContainer>
	);
}
