import { StrictMode } from 'react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import './main.css';
import App from './app';

const queryClient = new QueryClient({
	defaultOptions: {
		queries: {
			staleTime: Infinity,
		},
	},
});

export const AppElement = (
	<QueryClientProvider client={queryClient}>
		<StrictMode>
			<App />
			<ReactQueryDevtools initialIsOpen={false} buttonPosition="bottom-left" />
		</StrictMode>
	</QueryClientProvider>
);
