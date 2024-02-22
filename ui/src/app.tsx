import type { RouteObject } from 'react-router-dom';
import { HashRouter, Navigate, useRoutes } from 'react-router-dom';
import { Layout } from './components/layout/layout';
import { withErrorBoundary } from './components/router/withErrorBoundary';
import { OverviewComponent } from './pages/overview';
import './utils/i18n/setup';

const mainRoute: RouteObject[] = [
	{ path: 'overview/', Component: withErrorBoundary(OverviewComponent) },
	{ path: '/', element: <Navigate to="/overview" replace={true} /> },
];

function App() {
	return <Layout>{useRoutes(mainRoute)}</Layout>;
}

function AppProviders() {
	return (
		<HashRouter future={{ v7_startTransition: true }}>
			<App />
		</HashRouter>
	);
}

export default AppProviders;
