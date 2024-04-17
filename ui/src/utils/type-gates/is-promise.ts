export function isPromise<T>(t: T): t is Extract<T, PromiseLike<unknown>> {
	return typeof t === 'object' && t !== null && 'then' in t;
}
