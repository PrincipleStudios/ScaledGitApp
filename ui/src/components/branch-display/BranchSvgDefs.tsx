import { createContext, useContext, useId, useMemo } from 'react';

export type BranchCircleStyleContext = {
	idPrefix: string;
};
const styleContext = createContext<BranchCircleStyleContext | null>(null);

export function GraphSvgDefs({ children }: { children?: React.ReactNode }) {
	const idPrefix = useId();
	const styleContextValue = useMemo(
		(): BranchCircleStyleContext => ({ idPrefix }),
		[idPrefix],
	);
	return (
		<styleContext.Provider value={styleContextValue}>
			<pattern
				id={`${idPrefix}-vertical-bars`}
				x="0"
				y="0"
				width="4"
				height="4"
				patternUnits="userSpaceOnUse"
			>
				<rect x="1" width="2" y="0" height="4" fill="#fff" />
			</pattern>
			<mask
				id={`${idPrefix}-vertical-bars-mask`}
				x="0"
				y="0"
				width="4"
				height="4"
			>
				<rect
					x="-2000"
					y="-2000"
					width="4000"
					height="4000"
					fill={`url(#${idPrefix}-vertical-bars)`}
				/>
			</mask>
			{children}
		</styleContext.Provider>
	);
}

export function useGraphSvgStyleContext(): {
	noCommits: React.SVGProps<SVGCircleElement>;
} {
	const style = useContext(styleContext);
	if (!style)
		return {
			noCommits: {},
		};
	return {
		noCommits: { mask: `url(#${style.idPrefix}-vertical-bars-mask)` },
	};
}
