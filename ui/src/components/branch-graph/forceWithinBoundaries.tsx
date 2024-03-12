import { isNumber } from '../../utils/isNumber';
import type { ElementDimensions } from '../../utils/atoms/useResizeDetector';
import type { SimulationNodeDatum } from 'd3-force';

export function forceWithinBoundaries<TNode extends SimulationNodeDatum>(
	getSize: () => ElementDimensions,
	inset: number = 0,
) {
	let nodes: TNode[] = [];
	function update() {
		const { width = 0, height = 0 } = getSize();

		const minX = Math.min(...nodes.map((n) => n.x).filter(isNumber));
		const maxX = Math.max(...nodes.map((n) => n.x).filter(isNumber));
		const [offsetX, scaleX] = toOffsetScale(minX, maxX, width);

		const minY = Math.min(...nodes.map((n) => n.y).filter(isNumber));
		const maxY = Math.max(...nodes.map((n) => n.y).filter(isNumber));
		const [offsetY, scaleY] = toOffsetScale(minY, maxY, height);

		for (const node of nodes) {
			if (isNumber(node.x)) node.x = node.x * scaleX + offsetX;
			if (isNumber(node.y)) node.y = node.y * scaleY + offsetY;
		}
	}
	return Object.assign(update, {
		initialize(newNodes: TNode[]) {
			nodes = newNodes;
		},
	});

	function toOffsetScale(
		min: number,
		max: number,
		targetRange: number,
	): [offset: number, scale: number] {
		const actualRange = max - min;

		if (actualRange === Number.POSITIVE_INFINITY || targetRange <= 0)
			return [0, 1];
		return [
			Math.max(0, 0 - min + inset),
			Math.min(1, (targetRange - inset * 2) / actualRange),
		];
	}
}
