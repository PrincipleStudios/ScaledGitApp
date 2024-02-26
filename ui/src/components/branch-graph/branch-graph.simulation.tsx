import { useCallback, useEffect, useRef } from 'react';
import {
	forceCenter,
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

function forceHierarchy<TNode extends SimulationNodeDatum>(
	getX: (node: TNode) => number,
): Force<TNode, never> {
	let currentNodes: TNode[] = [];
	function update() {
		for (const node of currentNodes) {
			// node.fx = 0;
			node.x = getX(node);
		}
	}
	return Object.assign(update, {
		initialize(nodes: TNode[]) {
			currentNodes = nodes;
		},
	});
}

export function useBranchSimulation(upstreamData: UpstreamBranches) {
	const linkingForce = useRef(
		forceLink<WithAtom<BranchGraphNodeDatum>, WithAtom<BranchGraphLinkDatum>>(
			[],
		),
	);
	const centeringForce = useRef(forceCenter(150, 75));
	const simulationRef = useRef<BranchSimulation>();
	if (simulationRef.current === undefined) {
		simulationRef.current = forceSimulation<
			WithAtom<BranchGraphNodeDatum>,
			WithAtom<BranchGraphLinkDatum>
		>([])
			.force('link', linkingForce.current)
			.force('charge', forceManyBody().distanceMax(80).strength(-200))
			.force(
				'hierarchy',
				forceHierarchy<WithAtom<BranchGraphNodeDatum>>(
					(node) => node.depth * 100,
				),
			)
			.force('center', centeringForce.current);
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
		svgRefCallback: useCallback((svg: SVGSVGElement | null) => {
			if (svg) {
				centeringForce.current.x(svg.clientWidth / 2);
				centeringForce.current.y(svg.clientHeight / 2);
			}
		}, []),
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
