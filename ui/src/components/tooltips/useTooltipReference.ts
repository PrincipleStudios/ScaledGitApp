import { useStore } from 'jotai';
import { tooltipState } from './state';

type TooltipReferenceResult = Pick<
	React.DOMAttributes<HTMLElement | SVGElement>,
	'onMouseEnter' | 'onMouseLeave'
>;

export function useTooltipReference<TParams extends unknown[]>(
	elemFactory: (...params: TParams) => React.ReactNode | null,
): (...params: TParams) => TooltipReferenceResult {
	const store = useStore();

	return (...params) => ({
		onMouseEnter(ev) {
			store.set(tooltipState, {
				contents: elemFactory(...params),
				target: ev.target as HTMLElement | SVGElement,
			});
		},
		onMouseLeave(ev) {
			store.set(tooltipState, (prev) =>
				prev?.target === ev.target ? null : prev,
			);
		},
	});
}
