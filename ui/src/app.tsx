import type { RouteObject } from 'react-router-dom';
import { HashRouter, Navigate, useRoutes } from 'react-router-dom';
import { Layout, useHeader } from './components/layout';
import { withErrorBoundary, withSearchParamsValue } from './components/router';
import { AboutComponent } from './pages/about';
import { BranchDetailsComponent } from './pages/branch-details';
import { OverviewComponent } from './pages/overview';
import './utils/i18n/setup';

const withSearchParamsName = withSearchParamsValue('name');

const mainRoute: RouteObject[] = [
	{ path: 'overview/', Component: withErrorBoundary(OverviewComponent) },
	{ path: 'about/', Component: withErrorBoundary(AboutComponent) },
	{
		path: 'branch/',
		Component: withErrorBoundary(withSearchParamsName(BranchDetailsComponent)),
	},
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
