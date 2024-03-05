namespace PrincipleStudios.ScaledGitApp.Git;

public interface IGitCloneConfiguration
{
	string RemoteAlias { get; }
	string UpstreamBranchName { get; }
	string? ToLocalTrackingBranchName(string remoteBranchName);
}
