import { useRef } from 'react';
import {
	forceCenter,
	forceLink,
	forceManyBody,
	forceSimulation,
} from 'd3-force';
import type { BranchList, UpstreamBranches } from '../../generated/api/models';
import type {
	ForceLink,
	Simulation,
	SimulationLinkDatum,
	SimulationNodeDatum,
} from 'd3-force';

export type BranchGraphPresentationProps = {
	upstreamData: UpstreamBranches;
};

type BranchGraphSimulationDatum = {
	id: string;
	upstreamBranches: BranchList;
} & SimulationNodeDatum;
type BranchGraphLinkDatum = {
	upstreamBranchName: string;
	downstreamBranchName: string;
	source: BranchGraphSimulationDatum;
	target: BranchGraphSimulationDatum;
} & SimulationLinkDatum<BranchGraphSimulationDatum>;

export function BranchGraphPresentation({
	upstreamData,
}: BranchGraphPresentationProps) {
	const linkForce = useRef(
		forceLink<BranchGraphSimulationDatum, BranchGraphLinkDatum>([]),
	);
	const simulationRef = useRef(
		forceSimulation<BranchGraphSimulationDatum, BranchGraphLinkDatum>([])
			.force('link', linkForce.current)
			.force('charge', forceManyBody().distanceMax(80).strength(-100))
			.force('center', forceCenter(150, 75)),
	);

	const { nodes, links } = updateNodes(
		simulationRef.current,
		linkForce.current,
		upstreamData,
	);

	return (
		<svg>
			{links.map((link) => (
				<line
					key={`${link.downstreamBranchName}-${link.upstreamBranchName}`}
					x1={link.source.x}
					y1={link.source.y}
					x2={link.target.x}
					y2={link.target.y}
					strokeWidth={1}
					className="stroke-black"
				/>
			))}
			{nodes.map((node) => (
				<circle key={node.id} cx={node.x} cy={node.y} r={5} />
			))}
		</svg>
	);
}

function updateNodes(
	simulation: Simulation<BranchGraphSimulationDatum, BranchGraphLinkDatum>,
	linkForce: ForceLink<BranchGraphSimulationDatum, BranchGraphLinkDatum>,
	upstreamData: UpstreamBranches,
) {
	const oldNodes = simulation.nodes();
	const newNodes = upstreamData.map((entry) => {
		const newNode: BranchGraphSimulationDatum = Object.assign(
			oldNodes.find((n) => n.id === entry.name) ?? {},
			{
				id: entry.name,
				upstreamBranches: entry.upstream,
			},
		);

		return newNode;
	});
	simulation.nodes(newNodes);

	const oldLinks = linkForce.links();
	const newLinks = upstreamData.flatMap((entry) => {
		const target = newNodes.find((n) => n.id === entry.name);
		if (!target) throw new Error(`Unknown branch name: ${entry.name}`);
		return entry.upstream.map((upstreamBranch) => {
			const source = newNodes.find((n) => n.id === upstreamBranch.name);
			if (!source)
				throw new Error(`Unknown branch name: ${upstreamBranch.name}`);
			const newLink: BranchGraphLinkDatum = Object.assign(
				oldLinks.find(
					(n) =>
						n.downstreamBranchName === entry.name &&
						n.upstreamBranchName === upstreamBranch.name,
				) ?? {
					source,
					target,
				},
				{
					downstreamBranchName: entry.name,
					upstreamBranchName: upstreamBranch.name,
				},
			);
			return newLink;
		});
	});
	linkForce.links(newLinks);

	simulation.tick(1000);

	return { nodes: newNodes, links: newLinks };
}
