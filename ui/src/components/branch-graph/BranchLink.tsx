import { twMerge } from 'tailwind-merge';
import type { Branch, DetailedUpstreamBranch } from '@/generated/api/models';
import { isDetailed } from '../branch-display';
import { JotaiG, JotaiLine } from '../svg/atom-elements';
import type { BranchGraphLinkDatum } from './branch-graph.simulation';
import { useComputedLinkValues } from './useComputedLinkValues';

function isDetailedUpstream(
	branch: Branch | undefined,
): branch is DetailedUpstreamBranch {
	return branch ? 'behindCount' in branch : false;
}

export function BranchLink({ link }: { link: BranchGraphLinkDatum }) {
	const { transform, negativeLen } = useComputedLinkValues(link);

	const targetLink = isDetailed(link.target.data)
		? link.target.data.upstream.find((d) => d.name === link.source.id)
		: undefined;

	return (
		<JotaiG
			style={{ transform: transform }}
			className={twMerge(
				targetLink?.hasConflict
					? 'text-red-800 dark:text-red-400'
					: 'text-black dark:text-white',
				isDetailedUpstream(targetLink) && targetLink.behindCount > 0
					? 'stroke-2'
					: '',
			)}
		>
			<JotaiLine
				x1={negativeLen}
				y1={0}
				x2={-5}
				y2={0}
				className="stroke-current"
				strokeDasharray={isDetailedUpstream(targetLink) ? undefined : '10,3'}
			/>
			<path d="M-5,0l-5,3v-6z" className={'fill-current'} />
		</JotaiG>
	);
}
