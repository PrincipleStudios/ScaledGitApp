import { Fragment } from 'react';
import { Trans, useTranslation } from 'react-i18next';
import type { Location } from 'react-router-dom';
import { useLocation } from 'react-router-dom';
import { useSuspenseQueries, useSuspenseQuery } from '@tanstack/react-query';
import { BranchName } from '@/components/branch-display/BranchName';
import { BulletList, Container, Link, Section } from '@/components/common';
import { Prose } from '@/components/text';
import type { Branch, ConflictDetails } from '@/generated/api/models';
import { queries } from '@/utils/api/queries';

export function BranchConflictsSummary({ name }: { name: string[] }) {
	const conflictDetails = useSuspenseQuery(
		queries.getConflictDetails(name),
	).data;
	const branches = useSuspenseQueries({
		queries: conflictDetails.branches.map((branch) =>
			queries.getBranchDetails(branch.name),
		),
	}).map((result) => result.data);
	const location = useLocation();

	return (
		<BranchConflictsSummaryPresentation
			branches={branches}
			conflictDetails={conflictDetails.conflicts}
			location={location}
		/>
	);
}

export function BranchConflictsSummaryPresentation({
	branches,
	conflictDetails,
	location,
}: {
	branches: Branch[];
	conflictDetails: ConflictDetails[];
	location: Location;
}) {
	const { t } = useTranslation('branch-conflicts');
	return (
		<Container.Flow>
			<Section.SingleColumn>
				<Prose>
					<Trans
						i18nKey="conflict-analysis"
						t={t}
						components={{
							Branches: branchNamesList(branches),
						}}
					/>
				</Prose>
				<BulletList>
					{conflictDetails.map((conflict, index) => (
						<BulletList.Item
							key={conflict.branches.map((b) => b.name).join(',')}
						>
							<ConflictSummary conflict={conflict} />
							<Link
								to={{ ...location, pathname: `inspect/${index}` }}
								relative="route"
							>
								{t('conflict.inspect')}
							</Link>
						</BulletList.Item>
					))}
				</BulletList>
				<Link to={{ ...location, pathname: '/branch' }}>
					{t('to-branch-graph')}
				</Link>
			</Section.SingleColumn>
		</Container.Flow>
	);
}

export function ConflictSummary({ conflict }: { conflict: ConflictDetails }) {
	const { t } = useTranslation('branch-conflicts');
	return (
		<dl>
			<dt>{t('conflict.branches-label')}</dt>
			<dd>{branchNamesList(conflict.branches)}</dd>
			<dt>{t('conflict.messages-label')}</dt>
			<dd>
				<BulletList>
					{conflict.messages.map((msg, index) => (
						<BulletList.Item key={index}>{msg}</BulletList.Item>
					))}
				</BulletList>
			</dd>
			{conflict.candidateIntegrationBranch.length > 0 && (
				<>
					<dt>{t('conflict.suggested-integration')}</dt>
					<dd>{branchNamesList(conflict.candidateIntegrationBranch)}</dd>
				</>
			)}
		</dl>
	);
}

function branchNamesList(branches: Branch[]) {
	return (
		<>
			{branches.map((b) => (
				<Fragment key={b.name}>
					<BranchName data={b} />{' '}
				</Fragment>
			))}
		</>
	);
}
