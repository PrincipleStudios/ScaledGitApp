namespace PrincipleStudios.ScaledGitApp.Git;

public record GitCloneConfiguration(string GitRootDirectory, string RemoteName, string BaseUpstreamBranchName, IReadOnlyList<FetchMapping> FetchMapping) : IGitCloneConfiguration
{
	public string RemoteAlias => RemoteName;
	public string UpstreamBranchName => ToLocalTrackingBranchName(BaseUpstreamBranchName)
		?? throw new InvalidOperationException($"Upstream branch {BaseUpstreamBranchName} is not tracked. Ensure it is covered by the fetch refspec.");

	public string? ToLocalTrackingBranchName(string remoteBranchName)
	{
		var rawMapping = FetchMapping.Select(m => m.TryApply(remoteBranchName, out var result) ? result : null).Where(v => v != null).FirstOrDefault();
		if (rawMapping != null) return rawMapping;

		var fullyQualified = GitConventions.ToFullyQualifiedBranchName(remoteBranchName);
		return FetchMapping.Select(m => m.TryApply(fullyQualified, out var result) ? result : null).Where(v => v != null).FirstOrDefault();
	}
}
