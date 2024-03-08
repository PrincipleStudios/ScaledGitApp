import { useComputedAtom } from '@principlestudios/jotai-react-signals';
import { isAtom } from '@principlestudios/jotai-utilities/isAtom';
import { twMerge } from 'tailwind-merge';
import type { Atom } from 'jotai';
import type { ClassNameValue } from 'tailwind-merge';

type JotaiTwMergeParam = ClassNameValue | Atom<ClassNameValue>;

export function useTwMerge(...params: JotaiTwMergeParam[]) {
	return useComputedAtom((get) =>
		twMerge(...params.map((p) => (isAtom(p) ? get(p) : p))),
	);
}
