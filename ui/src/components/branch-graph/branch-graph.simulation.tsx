import { useRef } from 'react';
import {
	forceCollide,
	forceLink,
	forceManyBody,
	forceSimulation,
} from 'd3-force';
import { useStore, type Atom } from 'jotai';
import { atomWithImperativeProxy } from '../../utils/atoms/jotai-imperative-atom';
import { isNumber } from '../../utils/isNumber';
import { forceWithinBoundaries } from './forceWithinBoundaries';
import { neutralizeVelocity } from './neutralizeVelocity';
import { useAnimationFrame } from './useAnimationFrame';
import type { Branch, BranchConfiguration } from '../../generated/api/models';
import type { JotaiStore } from '../../utils/atoms/JotaiStore';
import type { ElementDimensions } from '../../utils/atoms/useResizeDetector';
import type {
	ForceLink,
	Simulation,
	SimulationLinkDatum,
	SimulationNodeDatum,
} from 'd3-force';

const maxUnknownBranchPerNodeCount = 5;
const maxUnknownBranchCount = 100;
// If the total number of branches goes over this number, do not display any non-detailed branches
const branchCountTolerance = 150;

export type WithAtom<T> = T & {
	atom: Atom<T>;
};
export type BranchInfo =
	| (Branch & {
			detailed: false;
	  })
	| (BranchConfiguration & { detailed: true });
export type BranchGraphNodeDatum = {
	id: string;
	data: BranchInfo;
} & SimulationNodeDatum;
export type BranchGraphLinkDatum = {
	id: string;
	upstreamBranchName: string;
	downstreamBranchName: string;
	source: WithAtom<BranchGraphNodeDatum>;
	target: WithAtom<BranchGraphNodeDatum>;
} & SimulationLinkDatum<BranchGraphNodeDatum>;
type BranchSimulation = Simulation<
	WithAtom<BranchGraphNodeDatum>,
	WithAtom<BranchGraphLinkDatum>
>;
type BranchLinkForce = ForceLink<
	WithAtom<BranchGraphNodeDatum>,
	WithAtom<BranchGraphLinkDatum>
>;

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

function average(values: number[]): number {
	if (values.length === 0) return NaN;
	return values
		.map((v) => v / values.length)
		.reduce((prev, next) => prev + next, 0);
}

function forceHierarchy(depthDistance: number) {
	let currentNodes: WithAtom<BranchGraphNodeDatum>[] = [];
	let links: WithAtom<BranchGraphLinkDatum>[] = [];
	const downstreamByNode = resettableMemo((node) =>
		links.filter((l) => l.target === node).map((l) => l.source),
	);
	const upstreamByNode = resettableMemo((node) =>
		links.filter((l) => l.source === node).map((l) => l.target),
	);
	function update(alpha: number) {
		for (const node of currentNodes) {
			if (isNumber(node.fx)) continue;
			const downstream = downstreamByNode.get(node);
			const upstream = upstreamByNode.get(node);
			const range = [
				Math.max(...downstream.map(toX).filter(isNumber)) + depthDistance,
				Math.min(...upstream.map(toX).filter(isNumber)) - depthDistance,
			].filter(isNotInfinity);
			if (range.length === 0) return;
			const targetX = average(range);

			const currentX = node.x ?? 0;
			const delta = targetX - currentX;
			const amount = delta * alpha;
			node.vx = (node.vx ?? 0) + amount;
		}
	}
	return Object.assign(update, {
		initialize(nodes: WithAtom<BranchGraphNodeDatum>[]) {
			currentNodes = nodes;
		},
		links(newLinks: WithAtom<BranchGraphLinkDatum>[]) {
			links = newLinks;
			downstreamByNode.clear();
			upstreamByNode.clear();
		},
	});
}

export function useBranchSimulation<T extends BranchConfiguration>(
	upstreamData: T[],
	size: Atom<ElementDimensions>,
) {
	const linkingForce = useRef(
		forceLink<WithAtom<BranchGraphNodeDatum>, WithAtom<BranchGraphLinkDatum>>(
			[],
		)
			.distance(40)
			.strength(0.5),
	);
	const hierarchyForce = useRef(forceHierarchy(40));
	const simulationRef = useRef<BranchSimulation>();
	const store = useStore();
	const getSize = () => store.get(size);
	if (simulationRef.current === undefined) {
		simulationRef.current = forceSimulation<
			WithAtom<BranchGraphNodeDatum>,
			WithAtom<BranchGraphLinkDatum>
		>([])
			.alphaDecay(0.01)
			.force('link', linkingForce.current)
			.force('collide', forceCollide(6))
			.force('spaceAround', forceManyBody().distanceMax(100).strength(-100))
			.force('sizing', forceWithinBoundaries(getSize))
			.force('hierarchy', hierarchyForce.current)
			.force('neutral-velocity', neutralizeVelocity());
	}

	const animator = useAnimationFrame(() => {
		if (!simulationRef.current) return false;
		simulationRef.current.tick();
		return simulationRef.current.alpha() >= simulationRef.current.alphaMin();
	});

	const { nodes, links } = updateNodes(
		store,
		simulationRef.current,
		linkingForce.current,
		hierarchyForce.current,
		upstreamData,
		getSize(),
	);

	restartSimulation();

	return {
		nodes,
		links,
		restartSimulation,
	};

	function restartSimulation() {
		simulationRef.current?.alpha(0.1);
		simulationRef.current?.restart();
		animator.restart();
	}
}

function toLookups(upstreamData: BranchConfiguration[]) {
	const configsByName = Object.fromEntries(
		upstreamData.map((e): [string, BranchInfo] => [
			e.name,
			{ ...e, detailed: true },
		]),
	);
	const dataLookup: Record<string, BranchInfo> = { ...configsByName };
	const extraBranches: Record<string, BranchInfo> = { ...configsByName };
	const configuredLinks: { upstream: string; downstream: string }[] = [];
	// fill in missing data - both missing upstream/downstream nodes and links
	const branchCount = upstreamData.length;
	let unknownBranchCount = 0;
	let currentNodeUnknownBranchCount = 0;
	for (const config of upstreamData) {
		currentNodeUnknownBranchCount = 0;
		for (const downstream of config.downstream) {
			tryAddUnknownBranch(downstream);
			tryAddLink(config.name, downstream.name);
		}
		for (const upstream of config.upstream) {
			tryAddUnknownBranch(upstream);
			tryAddLink(upstream.name, config.name);
		}
	}
	if (branchCount + unknownBranchCount < branchCountTolerance)
		Object.assign(dataLookup, extraBranches);

	return { dataLookup, configuredLinks };

	function tryAddUnknownBranch(branch: Branch) {
		if (dataLookup[branch.name] || extraBranches[branch.name]) return;
		if (unknownBranchCount >= maxUnknownBranchCount) return;
		if (currentNodeUnknownBranchCount >= maxUnknownBranchPerNodeCount) return;
		unknownBranchCount++;
		currentNodeUnknownBranchCount++;
		extraBranches[branch.name] = { ...branch, detailed: false };
	}
	function tryAddLink(u: string, d: string) {
		if (!configuredLinks.find((l) => l.upstream === u && l.downstream === d))
			configuredLinks.push({
				upstream: u,
				downstream: d,
			});
	}
}

function updateNodes(
	store: JotaiStore,
	simulation: BranchSimulation,
	linkForce: BranchLinkForce,
	hierarchyForce: {
		links: (newLinks: WithAtom<BranchGraphLinkDatum>[]) => void;
	},
	upstreamData: BranchConfiguration[],
	{ width = 0, height = 0 }: Pick<ElementDimensions, 'width' | 'height'>,
) {
	const { dataLookup, configuredLinks } = toLookups(upstreamData);

	// Updates nodes while creating atom proxy for animation
	const oldNodes = simulation.nodes();
	const newNodes = Object.values(dataLookup).map((entry) =>
		findOrCreate(
			store,
			oldNodes,
			(n) => n.id === entry.name,
			{
				x: width / 2,
				y: height / 2,
			},
			{
				id: entry.name,
				data: entry,
			},
		),
	);
	const nodeLookup = new Map<string, WithAtom<BranchGraphNodeDatum>>(
		newNodes.map((e) => [e.id, e] as const),
	);

	// Updates links while creating atom proxy for animation
	const oldLinks = linkForce.links();
	const newLinks = configuredLinks
		.map((entry) => {
			const target = nodeLookup.get(entry.downstream);
			const source = nodeLookup.get(entry.upstream);
			if (!target || !source) return null;
			const id = `${entry.downstream}-${entry.upstream}`;
			return findOrCreate(
				store,
				oldLinks,
				(n) => n.id === id,
				{
					source,
					target,
				},
				{
					id,
					downstreamBranchName: entry.downstream,
					upstreamBranchName: entry.upstream,
				},
			);
		})
		.filter((v): v is NonNullable<typeof v> => v !== null);

	// Updates the simulation and link force
	simulation.nodes(newNodes);
	linkForce.links(newLinks);
	hierarchyForce.links(newLinks);

	return { nodes: newNodes, links: newLinks };
}

// Finds or creates an item in the list with an atom produced by a proxy
function findOrCreate<T, TKeys extends keyof T>(
	store: JotaiStore,
	previous: WithAtom<T>[],
	match: (value: T) => boolean,
	initial: Pick<T, TKeys>,
	updates: Omit<T, TKeys>,
): WithAtom<T> {
	const found = previous.find(match);
	let result: WithAtom<T>;
	if (found) {
		Object.assign(found, updates);
		result = found;
	} else {
		const [original, atom] = atomWithImperativeProxy<WithAtom<T>>(
			{ ...initial, ...updates } as WithAtom<T>,
			store,
		);
		original.atom = atom;
		result = original;
	}
	return result;
}
