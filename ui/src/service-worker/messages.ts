import type { HubConnectionState } from '@microsoft/signalr';

export type RequestHubStateMessage = {
	type: 'requestHubState';
};
export type RequestReconnectMessage = {
	type: 'requestReconnect';
};
export type ForceDisconnectMessage = {
	type: 'forceDisconnect';
};
export type ForceReconnectMessage = {
	type: 'forceReconnect';
};

export type LogMessage = {
	type: 'log';
	args: readonly unknown[];
};

export type HubStatusMesage = {
	type: 'hubState';
	state: HubConnectionState;
};

export type MessageFromWindow =
	| RequestHubStateMessage
	| RequestReconnectMessage
	| ForceDisconnectMessage
	| ForceReconnectMessage;
export type MessageFromServiceWorker = LogMessage | HubStatusMesage;
