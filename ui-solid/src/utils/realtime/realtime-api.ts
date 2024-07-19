import {
	createContext,
	useContext,
	createSignal,
	Accessor,
	createRoot,
	createEffect,
} from 'solid-js';
import { HubConnectionState } from '@microsoft/signalr';
import type { QueryClient } from '@tanstack/solid-query';
import { neverEver } from '@/utils/never-ever';
import type {
	MessageFromServer,
	MessageFromApp,
} from '@/utils/realtime/messages';
import { realtimeApiEventTarget } from './implementation';

const reconnectStates = [
	HubConnectionState.Connecting,
	HubConnectionState.Reconnecting,
];

function subscribe(callback: (disposer: () => void) => void) {
	let dispose: () => void;
	createRoot((disposer) => {
		dispose = disposer;
		createEffect(() => callback(disposer));
	});
	return dispose!;
}

export function createRealtimeApi(queryClient: QueryClient): RealtimeApi {
	const [connectionState$, setConnectionState] = createSignal(
		HubConnectionState.Connecting,
	);

	realtimeApiEventTarget.addServerMessageHandler((data) => {
		void handleServiceMessage(data);
	});
	requestHubState();

	const result: RealtimeApi = {
		connectionState$,
		reconnect() {
			sendServiceMessage({ type: 'requestReconnect' });
			return new Promise<void>((resolve, reject) => {
				connectionState$;
				subscribe((dispose) => {
					const current = connectionState$();
					if (current === HubConnectionState.Connected) {
						resolve();
						dispose();
					}
					if (current === HubConnectionState.Disconnected) {
						reject();
						dispose();
					}
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
				if (connectionState$() === message.state) break;
				setConnectionState(message.state);
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
	readonly connectionState$: Accessor<HubConnectionState>;
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
