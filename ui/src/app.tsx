import type { RouteObject } from 'react-router-dom';
import { HashRouter, Navigate, useRoutes } from 'react-router-dom';
import { Layout, useHeader } from './components/layout';
import { withErrorBoundary } from './components/router/withErrorBoundary';
import { AboutComponent } from './pages/about';
import { OverviewComponent } from './pages/overview';
import './utils/i18n/setup';

const mainRoute: RouteObject[] = [
	{ path: 'overview/', Component: withErrorBoundary(OverviewComponent) },
	{ path: 'about/', Component: withErrorBoundary(AboutComponent) },
	{ path: '/', element: <Navigate to="/overview" replace={true} /> },
];

function App() {
	const header = useHeader();
	return <Layout header={header}>{useRoutes(mainRoute)}</Layout>;
}

function AppProviders() {
	return (
		<HashRouter future={{ v7_startTransition: true }}>
			<App />
		</HashRouter>
	);
}

export default AppProviders;
