import { withSignal, mapProperty } from '@principlestudios/jotai-react-signals';

export const JotaiSpan = withSignal('span', {
	className: mapProperty('className'),
});
