import { atom } from 'jotai';

type TooltipState = {
	/** JSX binding including its props for display */
	contents: React.ReactNode | null;
	/** The current element that the tooltip should be focused on */
	target: HTMLElement | SVGElement | null;
};

export const tooltipState = atom<TooltipState | null>(null);
