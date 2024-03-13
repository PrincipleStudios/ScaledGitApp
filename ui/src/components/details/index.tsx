export function Details({ children }: { children?: React.ReactNode }) {
	return <dl className="grid grid-cols-[auto,1fr] gap-4">{children}</dl>;
}
function DetailsEntry({
	label,
	children,
}: {
	label: React.ReactNode;
	children?: React.ReactNode;
}) {
	return (
		<>
			<dt className="col-start-1">{label}</dt>
			<dd className="col-start-2">{children}</dd>
		</>
	);
}
DetailsEntry.displayName = 'Details.Entry';
Details.Entry = DetailsEntry;
