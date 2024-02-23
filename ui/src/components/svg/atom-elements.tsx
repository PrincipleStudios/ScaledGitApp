import { withSignal, mapProperty } from '@principlestudios/jotai-react-signals';

export const JotaiCircle = withSignal('circle', {
	cx: (el) => (cx: number) => el.setAttribute('cx', cx.toFixed(5)),
	cy: (el) => (cy: number) => el.setAttribute('cy', cy.toFixed(5)),
	r: (el) => (r: number) => el.setAttribute('r', r.toFixed(5)),
	className: mapProperty('className'),
});

export const JotaiLine = withSignal('line', {
	x1: (el) => (x1: number) => el.setAttribute('x1', x1.toFixed(5)),
	y1: (el) => (y1: number) => el.setAttribute('y1', y1.toFixed(5)),
	x2: (el) => (x2: number) => el.setAttribute('x2', x2.toFixed(5)),
	y2: (el) => (y2: number) => el.setAttribute('y2', y2.toFixed(5)),
	strokeWidth: (el) => (strokeWidth: number) =>
		el.setAttribute('strokeWidth', strokeWidth.toFixed(5)),
	className: mapProperty('className'),
});
