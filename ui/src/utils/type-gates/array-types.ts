export function isArrayOfType<TInput, TOutput extends TInput>(
	t: Array<TInput>,
	typeGate: (target: TInput) => target is TOutput,
): t is Array<TOutput> {
	return !t.some((v) => !typeGate(v));
}
