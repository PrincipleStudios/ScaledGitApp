/* @refresh reload */
import { render } from 'solid-js/web';
import { QueryClient, QueryClientProvider } from '@tanstack/solid-query';
import { SolidQueryDevtools } from '@tanstack/solid-query-devtools';
import './index.css';
import App from './App';

const queryClient = new QueryClient({
	defaultOptions: {
		queries: {
			staleTime: Infinity,
		},
	},
});

render(
	() => (
		<QueryClientProvider client={queryClient}>
			<App />
			<SolidQueryDevtools />
		</QueryClientProvider>
	),
	document.getElementById('root') as HTMLElement,
);
