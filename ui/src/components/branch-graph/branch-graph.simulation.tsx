import { useEffect, useRef } from 'react';
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

export function useBranchSimulation(upstreamData: UpstreamBranches) {
	const linkingForce = useRef(
		forceLink<WithAtom<BranchGraphNodeDatum>, WithAtom<BranchGraphLinkDatum>>(
			[],
		),
	);
	const centeringForce = useRef(forceCenter(150, 75));
	const simulationRef = useRef(
		forceSimulation<
			WithAtom<BranchGraphNodeDatum>,
			WithAtom<BranchGraphLinkDatum>
		>([])
			.force('link', linkingForce.current)
			.force('charge', forceManyBody().distanceMax(80).strength(-100))
			.force('center', centeringForce.current),
	);

	useEffect(function runSimulation() {
		let cancelToken: null | number = null;
		function animate() {
			cancelToken = requestAnimationFrame(animate);
			simulationRef.current.tick();
		}
		animate();
		return () => {
			if (cancelToken !== null) cancelAnimationFrame(cancelToken);
		};
	}, []);

	const store = useStore();
	return updateNodes(
		store,
		simulationRef.current,
		linkingForce.current,
		upstreamData,
	);
}

function updateNodes(
	store: JotaiStore,
	simulation: BranchSimulation,
	linkForce: BranchLinkForce,
	upstreamData: UpstreamBranches,
) {
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
			},
		),
	);
	simulation.nodes(newNodes);

	const oldLinks = linkForce.links();
	const newLinks = upstreamData.flatMap((entry) => {
		const target = newNodes.find((n) => n.id === entry.name);
		if (!target) throw new Error(`Unknown branch name: ${entry.name}`);
		return entry.upstream.map((upstreamBranch) => {
			const source = newNodes.find((n) => n.id === upstreamBranch.name);
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
	linkForce.links(newLinks);

	return { nodes: newNodes, links: newLinks };
}

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
