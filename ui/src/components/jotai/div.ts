import { withSignal, mapProperty } from '@principlestudios/jotai-react-signals';

export const JotaiDiv = withSignal('div', {
	className: mapProperty('className'),
});
