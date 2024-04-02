import type { SimulationNodeDatum } from 'd3-force';
import type { ElementDimensions } from '@/utils/atoms/useResizeDetector';
import { isNumber } from '@/utils/isNumber';

export function forceWithinBoundaries<TNode extends SimulationNodeDatum>(
	getSize: () => ElementDimensions,
	inset: number = 10,
) {
	let nodes: TNode[] = [];
	function update() {
		const { width = 0, height = 0 } = getSize();

		const xNodes = nodes.filter((n) => !isNumber(n.fx));
		const yNodes = nodes.filter((n) => !isNumber(n.fy));

		const minX = Math.min(...xNodes.map((n) => n.x).filter(isNumber));
		const maxX = Math.max(...xNodes.map((n) => n.x).filter(isNumber));
		const [offsetX, scaleX] = toOffsetScale(minX, maxX, width, inset);

		const minY = Math.min(...yNodes.map((n) => n.y).filter(isNumber));
		const maxY = Math.max(...yNodes.map((n) => n.y).filter(isNumber));
		const [offsetY, scaleY] = toOffsetScale(minY, maxY, height, inset);

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
}

export function toOffsetScale(
	min: number,
	max: number,
	targetRange: number,
	inset: number,
): [offset: number, scale: number] {
	const actualRange = max - min;

	if (actualRange === Number.POSITIVE_INFINITY || targetRange <= 0)
		return [0, 1];
	const scale = Math.min(1, (targetRange - inset * 2) / actualRange);
	return [
		min < inset
			? inset - min * scale
			: max * scale > targetRange - inset
				? targetRange - inset - max * scale
				: 0,
		scale,
	];
}
