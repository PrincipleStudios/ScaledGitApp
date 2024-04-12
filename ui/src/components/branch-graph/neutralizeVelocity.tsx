import type { BranchGraphNodeDatum } from './branch-graph.simulation';

/** Keeps all nodes with a net-zero velocity */
export function neutralizeVelocity() {
	let currentNodes: BranchGraphNodeDatum[] = [];
	function update() {
		let vx = 0;
		let vy = 0;
		for (const node of currentNodes) {
			node.vx ??= 0;
			vx += node.vx;
			node.vy ??= 0;
			vy += node.vy;
		}

		for (const node of currentNodes) {
			node.vx = (node.vx ?? 0) - vx / currentNodes.length;
			node.vy = (node.vy ?? 0) - vy / currentNodes.length;
		}
	}
	return Object.assign(update, {
		initialize(nodes: BranchGraphNodeDatum[]) {
			currentNodes = nodes;
		},
	});
}
