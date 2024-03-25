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

export type GitFetchedMessage = {
	type: 'gitFetched';
};
export type HubStatusMesage = {
	type: 'hubState';
	state: HubConnectionState;
};

export type MessageFromApp =
	| RequestHubStateMessage
	| RequestReconnectMessage
	| ForceDisconnectMessage
	| ForceReconnectMessage;
export type MessageFromServer = GitFetchedMessage | HubStatusMesage;
