import { LayoutPresentation } from './layout.presentation';
import type { LayoutProps } from './layout.presentation';

export function Layout({ children }: LayoutProps) {
	return <LayoutPresentation>{children}</LayoutPresentation>;
}
