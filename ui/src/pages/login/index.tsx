import { useTranslation } from 'react-i18next';
import { useSuspenseQuery } from '@tanstack/react-query';
import { Container, ExternalLink, Section } from '../../components/common';
import { Prose } from '../../components/text';
import { queries } from '../../utils/api/queries';

export function LoginComponent({ returnUrl }: { returnUrl: string[] }) {
	const schemes = useSuspenseQuery(queries.getLoginSchemes);
	return <LoginPresentation schemes={schemes.data} returnUrl={returnUrl[0]} />;
}

export function LoginPresentation({
	schemes,
	returnUrl,
}: {
	schemes: string[];
	returnUrl: string | undefined;
}) {
	const { t } = useTranslation('login');
	return (
		<Container.Flow>
			<Section.SingleColumn>
				<Prose>{t('intro')}</Prose>
				{schemes.map((scheme) => (
					<ExternalLink
						key={scheme}
						href={`/challenge/${scheme}?returnUrl=${encodeURIComponent(returnUrl ?? '/')}`}
					>
						{t(`scheme.${scheme}.login`)}
					</ExternalLink>
				))}
			</Section.SingleColumn>
		</Container.Flow>
	);
}
