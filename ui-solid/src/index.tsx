/* @refresh reload */
import { QueryClient, QueryClientProvider } from '@tanstack/solid-query';
import { SolidQueryDevtools } from '@tanstack/solid-query-devtools';
import './index.css';
import App from './App';
import { setupRuntimeApi } from './runtime-api';
import {
	RealtimeApiProvider,
	createRealtimeApi,
} from './utils/realtime/realtime-api';

const queryClient = new QueryClient({
	defaultOptions: {
		queries: {
			staleTime: Infinity,
		},
	},
});

const realtimeApi = createRealtimeApi(queryClient);
setupRuntimeApi({ queryClient, realtimeApi });

export const AppElement = (
	<QueryClientProvider client={queryClient}>
		<RealtimeApiProvider value={realtimeApi}>
			<App />
			<SolidQueryDevtools initialIsOpen={false} buttonPosition="bottom-left" />
		</RealtimeApiProvider>
	</QueryClientProvider>
);
