import groupBy from 'lodash/groupBy';

export type QueryString = Record<string, string[]>;

export function parseSearchString(searchString: string): QueryString {
	if (!searchString.startsWith('?'))
		// not a valid search string
		return {};
	const keyValues = searchString
		.substring(1)
		.split('&')
		.map(
			(entry) => entry.split('=').map(decodeURIComponent) as [string, string],
		);
	const groupedByKey = groupBy(keyValues, ([key]) => key);
	const result = Object.fromEntries(
		Object.values(groupedByKey).map((entries) => [
			entries[0][0],
			entries.map(([, value]) => value),
		]),
	);
	return result;
}

export function toSearchString(queryValues: QueryString) {
	const entries = Object.entries(queryValues);
	if (entries.length === 0) return '';
	return (
		'?' +
		entries
			.flatMap(([key, values]) =>
				values.map(
					(value) => `${encodeURIComponent(key)}=${encodeURIComponent(value)}`,
				),
			)
			.join('&')
	);
}
