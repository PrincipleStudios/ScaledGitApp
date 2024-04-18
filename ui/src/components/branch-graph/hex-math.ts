// See https://www.redblobgames.com/grids/hexagons/ - using "pointy top" hexes

type PixelPoint = { x: number; y: number };
type Hex = { q: number; r: number };
type CubeHex = { q: number; r: number; s: number };

function cubeToAxial(cube: CubeHex): Hex {
	return { q: cube.q, r: cube.r };
}

function axialToCube(hex: Hex) {
	return { ...hex, s: -hex.q - hex.r };
}

function axialRound(hex: Hex) {
	return cubeToAxial(cubeRound(axialToCube(hex)));
}

function cubeRound(frac: CubeHex): CubeHex {
	let q = Math.round(frac.q);
	let r = Math.round(frac.r);
	let s = Math.round(frac.s);

	const qDiff = Math.abs(q - frac.q);
	const rDiff = Math.abs(r - frac.r);
	const sDiff = Math.abs(s - frac.s);

	if (qDiff > rDiff && qDiff > sDiff) q = -r - s;
	else if (rDiff > sDiff) r = -q - s;
	else s = -q - r;

	return { q, r, s };
}

export function pixelToHex(point: PixelPoint, gridSize: number) {
	const q = ((Math.sqrt(3) / 3) * point.x - (1 / 3) * point.y) / gridSize;
	const r = ((2 / 3) * point.y) / gridSize;
	return axialRound({ q, r });
}

export function hexToPixel(hex: Hex, gridSize: number): PixelPoint {
	const x = gridSize * (Math.sqrt(3) * hex.q + (Math.sqrt(3) / 2) * hex.r);
	const y = gridSize * ((3 / 2) * hex.r);
	return { x, y };
}
