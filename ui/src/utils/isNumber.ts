export function isNumber(n: number | null | undefined): n is number {
	return typeof n === 'number';
}
