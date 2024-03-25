import { createContext, useContext } from 'react';
import type {
	MessageFromServiceWorker,
	MessageFromWindow,
} from '@/service-worker/messages.ts';
import { neverEver } from '@/utils/never-ever.ts';
import { HubConnectionState } from '@microsoft/signalr';
import type { QueryClient } from '@tanstack/react-query';
import { atom, getDefaultStore } from 'jotai';
import { registerSW } from 'virtual:pwa-register';
import type { Atom } from 'jotai';

const reconnectStates = [
	HubConnectionState.Connecting,
	HubConnectionState.Reconnecting,
];

export function createRealtimeApi(queryClient: QueryClient): RealtimeApi {
	const store = getDefaultStore();
	const connectionState$ = atom(HubConnectionState.Connecting);

	registerSW({
		onRegisteredSW() {
			if (!navigator.serviceWorker?.controller) {
				// After a hard refresh, sometimes the `controller` doesn't load
				window.location.reload();
			}
			navigator.serviceWorker?.addEventListener('message', (event) => {
				void handleServiceMessage(event.data as MessageFromServiceWorker);
			});
			requestHubState();
		},
		onRegisterError(error: unknown) {
			console.error('Unable to register service worker', error);
			// TODO: pop up a modal that tells the user something went wrong badly
		},
	});

	const result: RealtimeApi = {
		connectionState$,
		reconnect() {
			sendServiceMessage({ type: 'requestReconnect' });
			return new Promise<void>((resolve, reject) => {
				store.sub(connectionState$, () => {
					const current = store.get(connectionState$);
					if (current === HubConnectionState.Connected) resolve();
					if (current === HubConnectionState.Disconnected) reject();
				});
			});
		},
		sendServiceMessage,
	};

	function requestHubState() {
		sendServiceMessage({ type: 'requestHubState' });
	}

	async function handleServiceMessage(message: MessageFromServiceWorker) {
		switch (message.type) {
			case 'gitFetched':
				await queryClient.invalidateQueries();
				break;
			case 'hubState':
				console.log('hubState', message.state);
				if (reconnectStates.includes(message.state))
					setTimeout(requestHubState, 500);
				if (store.get(connectionState$) === message.state) break;
				store.set(connectionState$, message.state);
				if (message.state === HubConnectionState.Connected)
					await queryClient.invalidateQueries();
				break;
			default:
				return neverEver(message);
		}
	}

	function sendServiceMessage(message: MessageFromWindow) {
		navigator.serviceWorker?.controller?.postMessage(message);
	}

	return result;
}

export interface RealtimeApi {
	readonly connectionState$: Atom<HubConnectionState>;
	reconnect(this: void): Promise<void>;
	sendServiceMessage(this: void, message: MessageFromWindow): void;
}

const context = createContext<RealtimeApi | null>(null);

export function useRealtimeApi() {
	const realtimeApiContext = useContext(context);
	if (!realtimeApiContext) throw new Error('No realtime api context provided');
	return realtimeApiContext;
}

export const RealtimeApiProvider = context.Provider;
