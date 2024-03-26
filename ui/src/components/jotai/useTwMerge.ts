import { useComputedAtom } from '@principlestudios/jotai-react-signals';
import { currentValue } from '@principlestudios/jotai-utilities/currentValue';
import type { Atom } from 'jotai';
import { twMerge } from 'tailwind-merge';
import type { ClassNameValue } from 'tailwind-merge';

type JotaiTwMergeParam = ClassNameValue | Atom<ClassNameValue>;

export function useTwMerge(...params: JotaiTwMergeParam[]) {
	return useComputedAtom((get) =>
		twMerge(...params.map((p) => currentValue(p, get))),
	);
}
