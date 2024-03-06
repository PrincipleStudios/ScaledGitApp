import { twMerge } from 'tailwind-merge';
import { JotaiG, JotaiLine } from '../svg/atom-elements';
import { useComputedLinkValues } from './useComputedLinkValues';
import type { BranchGraphLinkDatum, WithAtom } from './branch-graph.simulation';
import type {
	Branch,
	BranchConfiguration,
	BranchDetails,
	DetailedUpstreamBranch,
} from '../../generated/api/models';

export function BranchLink({
	link,
}: {
	link: WithAtom<BranchGraphLinkDatum<BranchConfiguration | BranchDetails>>;
}) {
	const { transform, negativeLen } = useComputedLinkValues(link);

	const targetLink = link.target.data.upstream.find(
		(d) => d.name === link.source.id,
	) as Branch | DetailedUpstreamBranch | undefined;

	return (
		<JotaiG style={{ transform: transform }}>
			<JotaiLine
				x1={negativeLen}
				y1={0}
				x2={0}
				y2={0}
				strokeWidth={1}
				className={
					targetLink && 'behindCount' in targetLink
						? twMerge(
								targetLink.hasConflict
									? 'stroke-red-800 dark:stroke-red-200'
									: 'stroke-black',
								targetLink.behindCount > 0 ? 'stroke-2' : '',
							)
						: 'stroke-black'
				}
			/>
			<path
				d="M-5,0l-5,3v-6z"
				className={
					targetLink && 'behindCount' in targetLink
						? targetLink.hasConflict
							? 'fill-red-800 dark:fill-red-200'
							: 'fill-black'
						: 'fill-black'
				}
			/>
		</JotaiG>
	);
}
