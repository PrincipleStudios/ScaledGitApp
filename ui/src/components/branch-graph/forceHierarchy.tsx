import type { SimulationNodeDatum } from 'd3-force';
import { isNumber } from '@/utils/isNumber';
import type {
	BranchGraphNodeDatum,
	BranchGraphLinkDatum,
} from './branch-graph.simulation';

function toX(node: SimulationNodeDatum) {
	return node.fx ?? node.x;
}
function isNotInfinity(n: number) {
	return n !== Number.POSITIVE_INFINITY && n !== Number.NEGATIVE_INFINITY;
}
function resettableMemo<TInput, TOutput>(toOutput: (input: TInput) => TOutput) {
	const cache = new Map<TInput, TOutput>();
	return {
		get(this: void, input: TInput) {
			if (cache.has(input)) return cache.get(input) as TOutput;
			const result = toOutput(input);
			cache.set(input, result);
			return result;
		},
		clear(this: void) {
			cache.clear();
		},
	};
}

export function forceHierarchy(depthDistance: number) {
	let currentNodes: BranchGraphNodeDatum[] = [];
	let links: BranchGraphLinkDatum[] = [];
	const upstreamByNode = resettableMemo((node) =>
		links.filter((l) => l.target === node).map((l) => l.source),
	);
	const downstreamByNode = resettableMemo((node) =>
		links.filter((l) => l.source === node).map((l) => l.target),
	);
	function update(alpha: number) {
		for (const node of currentNodes) {
			const downstream = downstreamByNode.get(node);
			const upstream = upstreamByNode.get(node);
			const range = [
				Math.min(...downstream.map(toX).filter(isNumber)) - depthDistance,
				Math.max(...upstream.map(toX).filter(isNumber)) + depthDistance,
			].filter(isNotInfinity);
			if (range.length === 0) return;
			const targetX = range[0];

			const currentX = node.fx ?? node.x ?? 0;
			const delta = targetX - currentX;
			const amount = delta * alpha;
			node.vx = (node.vx ?? 0) + amount;
			node.x = (node.x ?? 0) + amount;
		}
	}
	return Object.assign(update, {
		initialize(nodes: BranchGraphNodeDatum[]) {
			currentNodes = nodes;
		},
		links(newLinks: BranchGraphLinkDatum[]) {
			links = newLinks;
			downstreamByNode.clear();
			upstreamByNode.clear();
		},
	});
}
