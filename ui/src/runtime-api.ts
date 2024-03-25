import type { QueryClient } from '@tanstack/react-query';
import type { RealtimeApi } from './utils/realtime/realtime-api';

declare global {
	interface Window {
		/** A global api object for debugging */
		gitappApi: RuntimeApi;
	}
}

export type RuntimeApi = {
	disconnectHub(): void;
	reconnectHub(): void;
};
export function setupRuntimeApi({
	realtimeApi,
}: {
	queryClient: QueryClient;
	realtimeApi: RealtimeApi;
}) {
	window.gitappApi = {
		disconnectHub() {
			realtimeApi.sendServiceMessage({
				type: 'forceDisconnect',
			});
		},
		reconnectHub() {
			realtimeApi.sendServiceMessage({
				type: 'forceReconnect',
			});
		},
	};
}
