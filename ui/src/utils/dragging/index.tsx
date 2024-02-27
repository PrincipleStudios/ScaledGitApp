export type DragHandlerOptions<T extends Element, TState = void> = {
	onMouseDown(ev: React.MouseEvent<T>): false | TState;
	onMouseMove?(ev: MouseEvent & { currentTarget: T }, state: TState): void;
	onMouseUp(ev: MouseEvent & { currentTarget: T }, state: TState): void;
};

export function setupDragHandler<T extends Element, TState = void>(
	options: DragHandlerOptions<T, TState>,
) {
	// HTML's MouseEvent does not support strongly-typing the `currentTarget`, while React's does. Make up for this.
	type TypedMouseEvent = MouseEvent & { currentTarget: T };

	let state: TState;
	return {
		onMouseDown(ev: React.MouseEvent<T>) {
			const result = options.onMouseDown(ev);
			if (result !== false) {
				state = result;
				document.addEventListener('mousemove', onMouseMove, true);
				document.addEventListener('mouseup', onMouseUp, true);
			}
		},
	};

	function onMouseMove(ev: MouseEvent) {
		options.onMouseMove?.(ev as TypedMouseEvent, state);
	}

	function onMouseUp(ev: MouseEvent) {
		document.removeEventListener('mousemove', onMouseMove, true);
		document.removeEventListener('mouseup', onMouseUp, true);
		options.onMouseUp(ev as TypedMouseEvent, state);
	}
}
