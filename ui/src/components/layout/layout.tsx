import { LayoutPresentation } from './layout.presentation';
import type { LayoutProps } from './layout.presentation';

export function Layout(props: LayoutProps) {
	return <LayoutPresentation {...props} />;
}
