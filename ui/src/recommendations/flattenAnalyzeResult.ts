import { isAtom } from '@principlestudios/jotai-utilities/isAtom';
import { atom, type Atom } from 'jotai';
import { loadable } from 'jotai/utils';
import { type Loadable } from 'jotai/vanilla/utils/loadable';
import { isArrayOfType } from '@/utils/type-gates/array-types';
import { isPromise } from '@/utils/type-gates/is-promise';
import type {
	RecommendationOutput,
	RecommendationRuleAnalyzeResult,
} from './rule-base';

type Input = RecommendationRuleAnalyzeResult;
type RecommendationAtom = Atom<Loadable<RecommendationOutput>[]>;

const asLoadable = new WeakMap<Promise<unknown>, RecommendationAtom>();

type Flat<T> = T extends Array<infer TElem> ? TElem : T;
type FlatRecommendationRuleAnalyzeResult =
	Flat<RecommendationRuleAnalyzeResult>;

function isRecommendationOutput(
	v: FlatRecommendationRuleAnalyzeResult,
): v is RecommendationOutput {
	if (isPromise(v) || isAtom(v)) {
		return false;
	}
	v satisfies RecommendationOutput;
	return true;
}

function toLoaded<T>(data: Awaited<T>): Loadable<T> {
	return { state: 'hasData', data };
}

/** `RecommendationRule['analyze']` can return a wide variety of types for
 * usability. This function flattens the result into a single standard type. */
export function flattenAnalyzeResult(
	returnValue: Input,
): Loadable<RecommendationOutput>[] | RecommendationAtom {
	const asArray = Array.isArray(returnValue) ? returnValue : [returnValue];
	if (isArrayOfType(asArray, isRecommendationOutput)) {
		return asArray.map(toLoaded);
	}

	const loadableAtoms = asArray.map((entry) => {
		if (isPromise(entry)) {
			let result = asLoadable.get(entry);
			if (!result) {
				result = unwrapLoadableAtom(loadable(atom(entry)));
			}
			return result;
		} else if (isAtom(entry)) {
			return atom((get) => {
				const results = get<
					Loadable<RecommendationOutput[]> | Loadable<RecommendationOutput[]>[]
				>(entry);
				const asArray = Array.isArray(results) ? results : [results];
				return asArray;
			});
		}
		return entry;
	});

	return atom((get) => {
		return loadableAtoms.flatMap((entry) => {
			if (!isAtom(entry)) return [toLoaded(entry)];
			const currentValue =
				get<Loadable<RecommendationOutput | RecommendationOutput[]>[]>(entry);

			const result = currentValue.flatMap<Loadable<RecommendationOutput>>(
				(entry) => {
					if (entry.state !== 'hasData') return entry;
					if (!Array.isArray(entry.data)) return toLoaded(entry.data);
					return entry.data.map(toLoaded);
				},
			);
			return result;
		});
	});
}

function unwrapLoadableAtom(
	source: Extract<FlatRecommendationRuleAnalyzeResult, Atom<unknown>>,
): RecommendationAtom {
	return atom((get) => {
		const current = get<
			Loadable<RecommendationOutput[]> | Loadable<RecommendationOutput[]>[]
		>(source);
		const asArray = Array.isArray(current) ? current : [current];
		const flattened = asArray.flatMap<Loadable<RecommendationOutput>>(
			(entry) => (entry.state === 'hasData' ? entry.data.map(toLoaded) : entry),
		);
		return flattened;
	});
}
