import { StrictMode } from 'react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import App from './app';
import { setupRuntimeApi } from './runtime-api';
import {
	RealtimeApiProvider,
	createRealtimeApi,
} from './utils/realtime/realtime-api.ts';
import './main.css';

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
			<StrictMode>
				<App />
				<ReactQueryDevtools
					initialIsOpen={false}
					buttonPosition="bottom-left"
				/>
			</StrictMode>
		</RealtimeApiProvider>
	</QueryClientProvider>
);
