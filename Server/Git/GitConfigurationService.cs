namespace PrincipleStudios.ScaledGitApp.Git;

public sealed class GitConfigurationService(
	Lazy<Task<GitCloneConfiguration>> gitCloneConfigurationAccessor
) : IGitConfigurationService
{
	public GitConfigurationService(Func<Task<GitCloneConfiguration>> gitCloneConfiguration)
		: this(new Lazy<Task<GitCloneConfiguration>>(gitCloneConfiguration))
	{
	}

	public GitConfigurationService(GitCloneConfiguration gitCloneConfiguration)
		: this(() => Task.FromResult(gitCloneConfiguration))
	{
	}

	public async Task<string?> ToLocalTrackingBranchName(string remoteBranchName)
	{
		var config = await gitCloneConfigurationAccessor.Value;
		return config.ToLocalTrackingBranchName(remoteBranchName);
	}
}
