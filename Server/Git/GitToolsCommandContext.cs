using PrincipleStudios.ScaledGitApp.Commands;
using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.Git;

public sealed class GitToolsCommandContext : IGitToolsCommandContext
{
	private readonly IPowerShellInvoker pwsh;
	private readonly GitCloneConfiguration gitCloneConfiguration;

	public GitToolsCommandContext(IPowerShellInvoker pwsh, IGitToolsInvoker gitToolsInvoker, GitCloneConfiguration gitCloneConfiguration, ILogger logger)
	{
		this.pwsh = pwsh;
		this.gitCloneConfiguration = gitCloneConfiguration;
		GitToolsInvoker = gitToolsInvoker;
		PowerShellCommandInvoker = new InstanceCommandInvoker<IPowerShellCommandContext>(this, logger);
		GitToolsCommandInvoker = new InstanceCommandInvoker<IGitToolsCommandContext>(this, logger);
	}

	public string UpstreamBranchName => ToLocalTrackingBranchName(gitCloneConfiguration.UpstreamBranchName)
		?? throw new InvalidOperationException($"Upstream branch {gitCloneConfiguration.UpstreamBranchName} is not tracked. Ensure it is covered by the fetch refspec.");

	public string? ToLocalTrackingBranchName(string remoteBranchName)
	{
		var rawMapping = gitCloneConfiguration.FetchMapping.Select(m => m.TryApply(remoteBranchName, out var result) ? result : null).Where(v => v != null).FirstOrDefault();
		if (rawMapping != null) return rawMapping;

		var fullyQualified = GitConventions.ToFullyQualifiedBranchName(remoteBranchName);
		return gitCloneConfiguration.FetchMapping.Select(m => m.TryApply(fullyQualified, out var result) ? result : null).Where(v => v != null).FirstOrDefault();
	}

	public IPowerShellCommandInvoker PowerShellCommandInvoker { get; }

	public IGitToolsCommandInvoker GitToolsCommandInvoker { get; }

	public IGitToolsInvoker GitToolsInvoker { get; }

	IPowerShellInvoker IPowerShellCommandContext.PowerShellInvoker => pwsh;
}
