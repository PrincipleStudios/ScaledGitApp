import { withSignal, mapProperty } from '@principlestudios/jotai-react-signals';

export const JotaiG = withSignal('g', {
	className: mapProperty('className'),
});

export const JotaiCircle = withSignal('circle', {
	cx: (el) => (cx: string) => el.setAttribute('cx', cx),
	cy: (el) => (cy: string) => el.setAttribute('cy', cy),
	r: (el) => (r: string) => el.setAttribute('r', r),
	className: mapProperty('className'),
});

export const JotaiLine = withSignal('line', {
	x1: (el) => (x1: string) => el.setAttribute('x1', x1),
	y1: (el) => (y1: string) => el.setAttribute('y1', y1),
	x2: (el) => (x2: string) => el.setAttribute('x2', x2),
	y2: (el) => (y2: string) => el.setAttribute('y2', y2),
	strokeWidth: (el) => (strokeWidth: string) =>
		el.setAttribute('strokeWidth', strokeWidth),
	className: mapProperty('className'),
});
