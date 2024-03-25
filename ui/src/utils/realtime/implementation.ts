import { neverEver } from '@/utils/never-ever';
import { createRealtimeApiConnection } from '@/utils/realtime/realtime.signalr';
import type { HubConnection } from '@microsoft/signalr';
import { HubConnectionState } from '@microsoft/signalr';
import type {
	HubStatusMesage,
	MessageFromServer,
	MessageFromApp,
} from './messages';
/// <reference types="vite/client" />

const serverMessageHandlers: Array<(message: MessageFromServer) => void> = [];

export const realtimeApiEventTarget = {
	addServerMessageHandler(handler: (message: MessageFromServer) => void) {
		serverMessageHandlers.push(handler);
	},
	sendToServer(message: MessageFromApp) {
		handleMessageFromWindow(message);
	},
};

const gitHash: string = import.meta.env.VITE_GITHASH ?? 'HEAD';

const { connection, connectionPromise } =
	createRealtimeApiConnection(setupConnection);

function send(message: MessageFromServer) {
	for (const handler of serverMessageHandlers) {
		handler(message);
	}
}

void (async function () {
	await connectionPromise;
	void send(getHubStateMessage());
})();

function setupConnection(connection: HubConnection) {
	connection.off('GitHash');
	connection.on('GitHash', (serverGitHash) => {
		if (serverGitHash !== gitHash) {
			window.location.reload();
		}
	});

	connection.off('GitFetched');
	connection.on('GitFetched', () => {
		void send({ type: 'gitFetched' });
	});

	connection.onclose((err) => {
		console.error('onclose', err);
		void send(getHubStateMessage());
	});
	connection.onreconnecting((err) => {
		console.error('onreconnecting', err);
		void send(getHubStateMessage());
	});
	connection.onreconnected(() => {
		void send(getHubStateMessage());
	});
}

function handleMessageFromWindow(data: MessageFromApp) {
	switch (data.type) {
		case 'requestHubState':
			send(getHubStateMessage());
			return;
		case 'requestReconnect':
			if (connection.state === HubConnectionState.Disconnected) {
				setupConnection(connection);
				void connection.start().finally(() => {
					void send(getHubStateMessage());
				});
				void send(getHubStateMessage());
			}
			break;
		case 'forceDisconnect':
			forceDisconnect();
			break;
		case 'forceReconnect':
			forceReconnect();
			break;
		default:
			return neverEver(data);
	}
}

function forceReconnect() {
	// To trigger this, run this in the console:
	// gitappApi.reconnectHub()
	// This will allow you to see WS communication
	void (async function () {
		if (connection.state !== HubConnectionState.Disconnected) {
			await connection.stop();
		}
		setupConnection(connection);
		void connection.start().finally(() => {
			void send(getHubStateMessage());
		});
		void send(getHubStateMessage());
	})();
}

function forceDisconnect() {
	// To trigger this, run this in the console:
	// gitappApi.disconnectHub()
	if (connection.state !== HubConnectionState.Disconnected) {
		void (async function () {
			await connection.stop();
		})();
	}
}

function getHubStateMessage(): HubStatusMesage {
	return {
		type: 'hubState',
		state: connection.state,
	};
}
