import { JotaiG, JotaiLine } from '../svg/atom-elements';
import { useComputedValues } from './useComputedValues';
import type { BranchGraphLinkDatum, WithAtom } from './branch-graph.simulation';

export function BranchLink({ link }: { link: WithAtom<BranchGraphLinkDatum> }) {
	const { transform, negativeLen } = useComputedValues(link);
	return (
		<JotaiG style={{ transform: transform }}>
			<JotaiLine
				x1={negativeLen}
				y1={0}
				x2={0}
				y2={0}
				strokeWidth={1}
				className="stroke-black"
			/>
			<path d="M-5,0l-5,3v-6z" />
		</JotaiG>
	);
}
