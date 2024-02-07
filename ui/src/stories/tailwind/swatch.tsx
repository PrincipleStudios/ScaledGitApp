import { twMerge } from 'tailwind-merge';
import { colors } from './info';

export function Swatches() {
	return (
		<div className="sb-unstyled text-slate-400">
			{Object.entries(colors).map(([colorName, colorValue]) => (
				<Swatch
					colorName={colorName}
					colorValue={colorValue}
					key={colorName}
					className="m-2"
				/>
			))}
		</div>
	);
}

export function Swatch({
	className,
	colorName,
	colorValue,
}: {
	className?: string;
	colorName: string;
	colorValue: string;
}) {
	return (
		<div
			className={twMerge('inline-flex flex-col gap-1 items-center', className)}
		>
			<div
				className={twMerge('w-20 h-20 inline-block border border-black')}
				style={{ backgroundColor: colorValue }}
			/>
			<span className="text-xs">{colorName}</span>
			<span className="text-xs">{colorValue}</span>
		</div>
	);
}
