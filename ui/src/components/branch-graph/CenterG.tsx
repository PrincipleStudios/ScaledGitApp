import { useComputedAtom } from '@principlestudios/jotai-react-signals';
import { JotaiG } from '../svg/atom-elements';
import type { ElementDimensions } from '../../utils/atoms/useResizeDetector';
import type { Atom } from 'jotai';

export function CenterG({
	size,
	children,
}: {
	size: Atom<ElementDimensions>;
	children?: React.ReactNode;
}) {
	const transform = useComputedAtom((get) => {
		const { width, height } = get(size);
		return `translate(${(width ?? 0) / 2}px, ${(height ?? 0) / 2}px)`;
	});
	return <JotaiG style={{ transform }}>{children}</JotaiG>;
}
