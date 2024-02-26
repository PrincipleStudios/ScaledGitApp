import { useEffect, useRef } from 'react';
import {
	forceCenter,
	forceCollide,
	forceLink,
	forceManyBody,
	forceSimulation,
} from 'd3-force';
import { useStore, type Atom } from 'jotai';
import { atomWithImperativeProxy } from '../../utils/atoms/jotai-imperative-atom';
import type { BranchList, UpstreamBranches } from '../../generated/api/models';
import type { JotaiStore } from '../../utils/atoms/JotaiStore';
import type {
	Force,
	ForceLink,
	Simulation,
	SimulationLinkDatum,
	SimulationNodeDatum,
} from 'd3-force';

export type WithAtom<T> = T & {
	atom: Atom<T>;
};
export type BranchGraphNodeDatum = {
	id: string;
	color: string;
	upstreamBranches: BranchList;
	depth: number;
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

function forceHierarchy(
	depthDistance: number,
): Force<WithAtom<BranchGraphNodeDatum>, never> {
	let currentNodes: WithAtom<BranchGraphNodeDatum>[] = [];
	function update(alpha: number) {
		const allDepth = currentNodes.map((n) => n.depth);
		const minDepth = Math.min(Number.POSITIVE_INFINITY, ...allDepth);
		const maxDepth = Math.max(Number.NEGATIVE_INFINITY, ...allDepth);
		const avgDepth = (minDepth + maxDepth) / 2;
		for (const node of currentNodes) {
			if (node.fx) continue;
			node.vx =
				(node.vx ?? 0) +
				(depthDistance * (node.depth - avgDepth) - (node.x ?? 0)) *
					alpha *
					alpha;
		}
	}
	return Object.assign(update, {
		initialize(nodes: WithAtom<BranchGraphNodeDatum>[]) {
			currentNodes = nodes;
		},
	});
}

export function useBranchSimulation(upstreamData: UpstreamBranches) {
	const linkingForce = useRef(
		forceLink<WithAtom<BranchGraphNodeDatum>, WithAtom<BranchGraphLinkDatum>>(
			[],
		).distance((n) => Math.abs(n.source.depth - n.target.depth) * 80),
	);
	const simulationRef = useRef<BranchSimulation>();
	if (simulationRef.current === undefined) {
		simulationRef.current = forceSimulation<
			WithAtom<BranchGraphNodeDatum>,
			WithAtom<BranchGraphLinkDatum>
		>([])
			.force('link', linkingForce.current)
			.force('collide', forceCollide(6))
			.force('charge', forceManyBody().strength(0.001))
			.force('spaceAround', forceManyBody().distanceMax(80))
			.force('center', forceCenter())
			.force('hierarchy', forceHierarchy(100));
	}

	useEffect(function runSimulation() {
		let cancelToken: null | number = null;
		function animate() {
			cancelToken = requestAnimationFrame(animate);
			simulationRef.current?.tick();
		}
		animate();
		return () => {
			if (cancelToken !== null) cancelAnimationFrame(cancelToken);
		};
	}, []);

	const store = useStore();
	const { nodes, links } = updateNodes(
		store,
		simulationRef.current,
		linkingForce.current,
		upstreamData,
	);
	return {
		nodes,
		links,
		restartSimulation() {
			simulationRef.current?.alpha(0.5);
			simulationRef.current?.restart();
		},
	};
}

function updateNodes(
	store: JotaiStore,
	simulation: BranchSimulation,
	linkForce: BranchLinkForce,
	upstreamData: UpstreamBranches,
) {
	// Updates nodes while creating atom proxy for animation
	const oldNodes = simulation.nodes();
	const newNodes = upstreamData.map((entry) =>
		findOrCreate(
			store,
			oldNodes,
			(n) => n.id === entry.name,
			{},
			{
				id: entry.name,
				color: entry.color,
				upstreamBranches: entry.upstream,
				depth: 0,
			},
		),
	);
	const nodeLookup = new Map<string, WithAtom<BranchGraphNodeDatum>>(
		newNodes.map((e) => [e.id, e] as const),
	);

	// Updates links while creating atom proxy for animation
	const oldLinks = linkForce.links();
	const newLinks = upstreamData.flatMap((entry) => {
		const target = nodeLookup.get(entry.name);
		if (!target) throw new Error(`Unknown branch name: ${entry.name}`);
		return entry.upstream.map((upstreamBranch) => {
			const source = nodeLookup.get(upstreamBranch.name);
			if (!source)
				throw new Error(`Unknown branch name: ${upstreamBranch.name}`);
			const id = `${entry.name}-${upstreamBranch.name}`;
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
					downstreamBranchName: entry.name,
					upstreamBranchName: upstreamBranch.name,
				},
			);
		});
	});

	// Sets the node depth. TODO: double-check my algorithm, this is inefficient
	const depth: Record<string, number> = Object.fromEntries(
		upstreamData.map((e) => [e.name, 0] as const),
	);
	for (let i = 0; i < newNodes.length; i++) {
		for (let j = 0; j < newNodes.length; j++) {
			for (let k = 0; k < newNodes[j].upstreamBranches.length; k++) {
				depth[newNodes[j].id] = Math.max(
					depth[newNodes[j].upstreamBranches[k].name] + 1,
					depth[newNodes[j].id],
				);
			}
		}
	}
	for (let i = 0; i < newNodes.length - 1; i++) {
		if (newNodes[i].depth !== depth[newNodes[i].id])
			newNodes[i].depth = depth[newNodes[i].id];
	}

	// Updates the simulation and link force
	simulation.nodes(newNodes);
	linkForce.links(newLinks);

	return { nodes: newNodes, links: newLinks };
}

// Finds or creates an item in the list with an atom produced by a proxy
function findOrCreate<TPartial, T extends TPartial>(
	store: JotaiStore,
	previous: WithAtom<T>[],
	match: (value: T) => boolean,
	initial: TPartial,
	updates: Omit<T, keyof TPartial>,
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
