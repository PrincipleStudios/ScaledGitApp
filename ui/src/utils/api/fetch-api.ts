import { toFetchApi } from '@principlestudios/openapi-codegen-typescript-fetch';
import operations from '../../generated/api/operations';

export const api = toFetchApi(operations, async (url, req) => {
	const result = await fetch(url, req);
	if (result.status === 401) {
		const currentLocation = toPathQueryHash(window.location);
		if (!currentLocation.startsWith('/#/login?'))
			window.location.href = `/#/login?returnUrl=${encodeURIComponent(toPathQueryHash(window.location))}`;

		// redirecting; cannot be reached
		throw new Error();
	}
	return result;
});

function toPathQueryHash(location: Location) {
	return location.pathname + location.search + location.hash;
}
