import type { HubConnection } from '@microsoft/signalr';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';

function abortPromise(signal: AbortSignal) {
	return new Promise((never, reject) =>
		signal.addEventListener('abort', () => reject()),
	);
}

function firstCompleted<T>(promises: Array<Promise<T>>) {
	return new Promise<T>((resolve, reject) =>
		promises.forEach((p) => void p.then(resolve, reject)),
	);
}

export function getConnection(
	setup: (connection: HubConnection) => void,
	signal: AbortSignal,
) {
	const hub = new HubConnectionBuilder()
		.withUrl(new URL('/hub', self.location.href).toString())
		.withAutomaticReconnect()
		.configureLogging(LogLevel.Information)
		.build();
	setup(hub);

	return [
		hub,
		(async () => {
			await firstCompleted([
				abortPromise(signal).catch(() => hub.stop()),
				hub.start(),
			]).catch((err) => {
				console.error(err);
			});
			return hub;
		})(),
	] as const;
}

export function createRealtimeApiConnection(
	setup: (connection: HubConnection) => void,
) {
	const cancellation = new AbortController();
	const [connection, connectionPromise] = getConnection(
		setup,
		cancellation.signal,
	);

	return {
		cancellation,
		connectionPromise,
		connection,
	};
}
