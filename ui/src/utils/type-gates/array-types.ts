export function isArrayOfType<TInput, TOutput extends TInput>(
	t: TInput[],
	typeGate: (target: TInput) => target is TOutput,
): t is TOutput[] {
	return !t.some((v) => !typeGate(v));
}
