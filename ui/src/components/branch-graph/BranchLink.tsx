import { twMerge } from 'tailwind-merge';
import { JotaiG, JotaiLine } from '../svg/atom-elements';
import { useComputedLinkValues } from './useComputedLinkValues';
import type { BranchGraphLinkDatum, WithAtom } from './branch-graph.simulation';
import type {
	Branch,
	DetailedUpstreamBranch,
} from '../../generated/api/models';

function isDetailedUpstream(
	branch: Branch | undefined,
): branch is DetailedUpstreamBranch {
	return branch ? 'behindCount' in branch : false;
}

export function BranchLink({ link }: { link: WithAtom<BranchGraphLinkDatum> }) {
	const { transform, negativeLen } = useComputedLinkValues(link);

	const targetLink = link.target.data.detailed
		? link.target.data.upstream.find((d) => d.name === link.source.id)
		: undefined;

	return (
		<JotaiG style={{ transform: transform }}>
			<JotaiLine
				x1={negativeLen}
				y1={0}
				x2={-5}
				y2={0}
				strokeWidth={1}
				className={
					isDetailedUpstream(targetLink)
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
					isDetailedUpstream(targetLink) && targetLink.hasConflict
						? 'fill-red-800 dark:fill-red-200'
						: 'fill-black'
				}
			/>
		</JotaiG>
	);
}
