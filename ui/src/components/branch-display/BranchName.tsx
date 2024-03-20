import { InlineText } from '../common';
import { FullSizeSvg } from '../svg/full-size-svg';
import { BranchSvgCircle } from './BranchSvgCircle';
import { useActiveBranchOnHover } from './useActiveBranchOnHover';
import type { BranchInfo } from './types';

export function BranchName({ data }: { data: BranchInfo }) {
	const onHover = useActiveBranchOnHover(data);
	return (
		<InlineText {...onHover}>
			<InlineText.Icon className="inline-block w-4 h-4 self-center">
				<FullSizeSvg>
					<g style={{ translate: '0.5rem 0.5rem' }}>
						<BranchSvgCircle data={data} />
					</g>
				</FullSizeSvg>
			</InlineText.Icon>
			{data.name}
		</InlineText>
	);
}
