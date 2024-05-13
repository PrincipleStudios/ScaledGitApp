import type { RouteObject } from 'react-router-dom';
import { HashRouter, Navigate, useRoutes } from 'react-router-dom';
import { Layout, useHeader } from './components/layout';
import { withErrorBoundary, withSearchParamsValue } from './components/router';
import { AboutComponent } from './pages/about';
import { BranchConflictsComponent } from './pages/branch-conflicts';
import { BranchDetailsComponent } from './pages/branch-details';
import { LoginComponent } from './pages/login';
import { OverviewComponent } from './pages/overview';
import './utils/i18n/setup';
import { Modals } from './utils/modal';

const withSearchParamsName = withSearchParamsValue('name');
const withSearchParamsReturnUrl = withSearchParamsValue('returnUrl');

const mainRoute: RouteObject[] = [
	{
		path: 'login/',
		Component: withErrorBoundary(withSearchParamsReturnUrl(LoginComponent)),
	},
	{ path: 'overview/', Component: withErrorBoundary(OverviewComponent) },
	{ path: 'about/', Component: withErrorBoundary(AboutComponent) },
	{
		path: 'branch/',
		Component: withErrorBoundary(withSearchParamsName(BranchDetailsComponent)),
	},
	{
		path: 'branch/conflicts',
		Component: withErrorBoundary(
			withSearchParamsName(BranchConflictsComponent),
		),
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
			<Modals />
		</HashRouter>
	);
}

export default AppProviders;
