import { createContext, useContext } from 'react';
import { neverEver } from '@/utils/never-ever.ts';
import type {
	MessageFromServer,
	MessageFromApp,
} from '@/utils/realtime/messages';
import { HubConnectionState } from '@microsoft/signalr';
import type { QueryClient } from '@tanstack/react-query';
import { atom, getDefaultStore } from 'jotai';
import { realtimeApiEventTarget } from './implementation';
import type { Atom } from 'jotai';

const reconnectStates = [
	HubConnectionState.Connecting,
	HubConnectionState.Reconnecting,
];

export function createRealtimeApi(queryClient: QueryClient): RealtimeApi {
	const store = getDefaultStore();
	const connectionState$ = atom(HubConnectionState.Connecting);

	realtimeApiEventTarget.addServerMessageHandler((data) => {
		void handleServiceMessage(data);
	});
	requestHubState();

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

	async function handleServiceMessage(message: MessageFromServer) {
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

	function sendServiceMessage(message: MessageFromApp) {
		realtimeApiEventTarget.sendToServer(message);
	}

	return result;
}

export interface RealtimeApi {
	readonly connectionState$: Atom<HubConnectionState>;
	reconnect(this: void): Promise<void>;
	sendServiceMessage(this: void, message: MessageFromApp): void;
}

const context = createContext<RealtimeApi | null>(null);

export function useRealtimeApi() {
	const realtimeApiContext = useContext(context);
	if (!realtimeApiContext) throw new Error('No realtime api context provided');
	return realtimeApiContext;
}

export const RealtimeApiProvider = context.Provider;
