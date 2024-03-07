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
import type { BranchConfiguration } from '../../generated/api/models';
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
export type BranchGraphNodeDatum<
	T extends BranchConfiguration = BranchConfiguration,
> = {
	id: string;
	depth: number;
	data: T;
} & SimulationNodeDatum;
export type BranchGraphLinkDatum<
	T extends BranchConfiguration = BranchConfiguration,
> = {
	id: string;
	upstreamBranchName: string;
	downstreamBranchName: string;
	source: WithAtom<BranchGraphNodeDatum<T>>;
	target: WithAtom<BranchGraphNodeDatum<T>>;
} & SimulationLinkDatum<BranchGraphNodeDatum<T>>;
type BranchSimulation<T extends BranchConfiguration = BranchConfiguration> =
	Simulation<
		WithAtom<BranchGraphNodeDatum<T>>,
		WithAtom<BranchGraphLinkDatum<T>>
	>;
type BranchLinkForce<T extends BranchConfiguration = BranchConfiguration> =
	ForceLink<
		WithAtom<BranchGraphNodeDatum<T>>,
		WithAtom<BranchGraphLinkDatum<T>>
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
			const targetX = depthDistance * (node.depth - avgDepth);
			const currentX = node.x ?? 0;
			node.vx = (node.vx ?? 0) + (targetX - currentX) * alpha;
		}
	}
	return Object.assign(update, {
		initialize(nodes: WithAtom<BranchGraphNodeDatum>[]) {
			currentNodes = nodes;
		},
	});
}

export function useBranchSimulation<T extends BranchConfiguration>(
	upstreamData: T[],
) {
	const linkingForce = useRef(
		forceLink<
			WithAtom<BranchGraphNodeDatum<T>>,
			WithAtom<BranchGraphLinkDatum<T>>
		>([]).distance((n) => Math.abs(n.source.depth - n.target.depth) * 80),
	);
	const simulationRef = useRef<BranchSimulation<T>>();
	if (simulationRef.current === undefined) {
		simulationRef.current = forceSimulation<
			WithAtom<BranchGraphNodeDatum<T>>,
			WithAtom<BranchGraphLinkDatum<T>>
		>([])
			.force('link', linkingForce.current)
			.force('collide', forceCollide(6))
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

	restartSimulation();

	return {
		nodes,
		links,
		restartSimulation,
	};

	function restartSimulation() {
		simulationRef.current?.alpha(0.1);
		simulationRef.current?.restart();
	}
}

function updateNodes<T extends BranchConfiguration>(
	store: JotaiStore,
	simulation: BranchSimulation<T>,
	linkForce: BranchLinkForce<T>,
	upstreamData: T[],
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
				data: entry,
				depth: 0,
			},
		),
	);
	const nodeLookup = new Map<string, WithAtom<BranchGraphNodeDatum<T>>>(
		newNodes.map((e) => [e.id, e] as const),
	);

	// Updates links while creating atom proxy for animation
	const oldLinks = linkForce.links();
	const newLinks = upstreamData
		.flatMap((entry) => {
			const target = nodeLookup.get(entry.name);
			if (!target) return null;
			return entry.upstream.map((upstreamBranch) => {
				const source = nodeLookup.get(upstreamBranch.name);
				if (!source) return null;
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
		})
		.filter((v): v is NonNullable<typeof v> => v !== null);

	// Sets the node depth. TODO: double-check my algorithm, this is inefficient
	const depth: Record<string, number> = Object.fromEntries(
		upstreamData.map((e) => [e.name, 0] as const),
	);
	for (let i = 0; i < newNodes.length; i++) {
		for (let j = 0; j < newNodes.length; j++) {
			for (let k = 0; k < newNodes[j].data.upstream.length; k++) {
				if (newNodes[j].data.upstream[k].name in depth)
					depth[newNodes[j].id] = Math.max(
						depth[newNodes[j].data.upstream[k].name] + 1,
						depth[newNodes[j].id],
					);
			}
		}
	}
	for (let i = 0; i < newNodes.length; i++) {
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
