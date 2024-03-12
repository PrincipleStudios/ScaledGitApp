import { JotaiDiv } from '../../jotai/div';
import { JotaiLabel } from '../../jotai/label';
import { JotaiSpan } from '../../jotai/span';
import { useTwMerge } from '../../jotai/useTwMerge';
import type { Atom } from 'jotai';

export type FieldProps = React.ComponentProps<typeof JotaiLabel> & {
	noLabel?: boolean;
	labelClassName?: string | Atom<string>;
	labelChildren?: React.ReactNode;
	contentsClassName?: string | Atom<string>;
};

export function Field({
	noLabel,
	className,
	labelClassName,
	labelChildren,
	contentsClassName,
	children,
	...props
}: FieldProps) {
	const classNameAtom = useTwMerge('group', className);
	const labelClassNameAtom = useTwMerge(
		'group-focus-within:font-bold text-sm transition-all pt-2',
		labelClassName,
	);
	const contentsClassNameAtom = useTwMerge('block', contentsClassName);

	const Container = noLabel ? JotaiSpan : JotaiLabel;

	return (
		<Container className={classNameAtom} {...props}>
			<JotaiSpan className={labelClassNameAtom}>{labelChildren}</JotaiSpan>
			<JotaiDiv className={contentsClassNameAtom}>{children}</JotaiDiv>
		</Container>
	);
}
