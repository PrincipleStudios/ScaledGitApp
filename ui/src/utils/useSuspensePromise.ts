import memoize from 'lodash/memoize';

function baseWrapPromise<T>(promise: Promise<T>) {
	let status: 'pending' | 'error' | 'success' = 'pending';
	let error: unknown;
	let response: T;

	const suspender = promise.then(
		(res) => {
			status = 'success';
			response = res;
		},
		(err) => {
			status = 'error';
			error = err;
		},
	);

	const handler = {
		pending: () => {
			throw suspender;
		},
		error: () => {
			console.log(error);
			throw error;
		},
		success: () => response,
	};

	const read = () => {
		return handler[status]();
	};

	return { read };
}
const wrapPromise = memoize(baseWrapPromise);

export function useSuspensePromise<T>(target: Promise<T>) {
	const result = wrapPromise(target);
	return result.read();
}
