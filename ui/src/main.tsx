import { StrictMode } from 'react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import './main.css';

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
			<main>Hello, world!</main>
			<ReactQueryDevtools initialIsOpen={false} buttonPosition="bottom-left" />
		</StrictMode>
	</QueryClientProvider>
);
