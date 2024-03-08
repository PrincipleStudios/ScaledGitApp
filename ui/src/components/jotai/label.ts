import { withSignal, mapProperty } from '@principlestudios/jotai-react-signals';

export const JotaiLabel = withSignal('label', {
	className: mapProperty('className'),
});
