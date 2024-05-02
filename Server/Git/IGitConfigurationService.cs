namespace PrincipleStudios.ScaledGitApp.Git;

public interface IGitConfigurationService
{
	Task<string?> ToLocalTrackingBranchName(string remoteBranchName);
}
