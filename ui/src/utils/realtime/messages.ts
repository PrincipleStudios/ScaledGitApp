import type { HubConnectionState } from '@microsoft/signalr';

/** Send this message to the realtime implementation to get the current hub
 * state */
export type RequestHubStateMessage = {
	type: 'requestHubState';
};
/** Send this message to initiate reconnection when the hub is disconnected */
export type RequestReconnectMessage = {
	type: 'requestReconnect';
};
/** Send this message to disconnect from the hub without reconnecting */
export type ForceDisconnectMessage = {
	type: 'forceDisconnect';
};
/** Send this message to disconnect (if necessary) and reconnect to the hub */
export type ForceReconnectMessage = {
	type: 'forceReconnect';
};

/** Sent from the realtime implementation when a `git fetch` has completed */
export type GitFetchedMessage = {
	type: 'gitFetched';
};
/** Sent from the realtime implementation when the hub's state has changed */
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
