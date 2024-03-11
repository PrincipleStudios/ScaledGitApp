import { useEffect, useMemo, useRef } from 'react';

export function useAnimationFrame(onFrame: () => boolean) {
	const onFrameRef = useRef(onFrame);
	onFrameRef.current = onFrame;

	const animator = useMemo(() => {
		let cancelToken: null | number = null;
		function animate() {
			if (onFrameRef.current()) {
				cancelToken = requestAnimationFrame(animate);
			}
		}
		animate();

		function cancel() {
			if (cancelToken !== null) cancelAnimationFrame(cancelToken);
		}

		return {
			cancel,
			restart: () => {
				cancel();
				cancelToken = requestAnimationFrame(animate);
			},
		};
	}, []);

	useEffect(() => {
		animator.restart();
		return animator.cancel;
	}, [animator]);

	return animator;
}
