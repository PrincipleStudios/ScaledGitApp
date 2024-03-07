export type DragHandlerOptions<T extends Element, TState = void> = {
	onMouseDown(ev: React.MouseEvent<T>): false | TState;
	onMouseMove?(
		ev: MouseEvent & { currentTarget: T },
		state: TState,
		info: { totalMovement: number },
	): void;
	onMouseUp(
		ev: MouseEvent & { currentTarget: T },
		state: TState,
		info: { totalMovement: number },
	): void;
};

export function setupDragHandler<T extends Element, TState = void>(
	options: DragHandlerOptions<T, TState>,
) {
	// HTML's MouseEvent does not support strongly-typing the `currentTarget`, while React's does. Make up for this.
	type TypedMouseEvent = MouseEvent & { currentTarget: T };

	let state: TState;
	let totalMovement = 0;
	let lastClientX = 0;
	let lastClientY = 0;
	return {
		onMouseDown(ev: React.MouseEvent<T>) {
			const result = options.onMouseDown(ev);
			if (result !== false) {
				totalMovement = 0;
				lastClientX = ev.clientX;
				lastClientY = ev.clientY;
				state = result;
				document.addEventListener('mousemove', onMouseMove, true);
				document.addEventListener('mouseup', onMouseUp, true);
			}
		},
	};

	function onMouseMove(ev: MouseEvent) {
		trackMovement(ev);
		options.onMouseMove?.(ev as TypedMouseEvent, state, { totalMovement });
	}

	function onMouseUp(ev: MouseEvent) {
		trackMovement(ev);
		document.removeEventListener('mousemove', onMouseMove, true);
		document.removeEventListener('mouseup', onMouseUp, true);
		options.onMouseUp(ev as TypedMouseEvent, state, { totalMovement });
	}

	function trackMovement(ev: MouseEvent) {
		const x = lastClientX - ev.clientX;
		const y = lastClientY - ev.clientY;
		totalMovement += Math.sqrt(x * x + y * y);
		lastClientX = ev.clientX;
		lastClientY = ev.clientY;
	}
}
