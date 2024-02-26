import { HeaderPresentation } from './header.presentation';
import type { HeaderProps } from './header.presentation';

export function useHeader(): React.ComponentType<HeaderProps> {
	return HeaderPresentation;
}
