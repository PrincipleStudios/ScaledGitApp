import { useBranchListing } from './branch-listing';
import styles from './overview.module.css';

export function OverviewComponent() {
	const BranchListing = useBranchListing();
	return (
		<div className={styles.container}>
			<BranchListing />
		</div>
	);
}
