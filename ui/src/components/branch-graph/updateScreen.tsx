import type { BranchGraphNodeDatum } from './branch-graph.simulation';
import { hexToPixel, pixelToHex } from './hex-math';

const gridSize = 10;

export function updateScreen(nodes: BranchGraphNodeDatum[]) {
	// TODO: track occupied hex locations and prevent multiple in same location
	const occupied = new Set<string>();
	for (const d of nodes) {
		d.x ??= 0;
		d.y ??= 0;
		const hex = pixelToHex({ x: d.x, y: d.y }, gridSize);
		const hexId = `${hex.q},${hex.r}`;
		if (!occupied.has(hexId)) {
			occupied.add(hexId);
			const grid = hexToPixel(hex, gridSize);

			d.screenX += (grid.x - d.screenX) * 0.2;
			d.screenY += (grid.y - d.screenY) * 0.2;

			d.x += (grid.x - d.x) * 0.05;
			d.y += (grid.y - d.y) * 0.05;
		} else {
			d.screenX = d.x;
			d.screenY = d.y;
		}
	}
}
