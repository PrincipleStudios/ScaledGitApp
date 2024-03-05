using PrincipleStudios.ScaledGitApp.Commands;
using PrincipleStudios.ScaledGitApp.ShellUtilities;
using System.Management.Automation;

namespace PrincipleStudios.ScaledGitApp.Git;

public sealed class GitToolsCommandContext : IGitToolsCommandContext, IGitToolsInvoker
{
	private readonly IPowerShellInvoker pwsh;
	private readonly GitOptions gitOptions;
	private readonly GitCloneConfiguration gitCloneConfiguration;

	public GitToolsCommandContext(IPowerShellInvoker pwsh, GitOptions gitOptions, GitCloneConfiguration gitCloneConfiguration, ILogger logger)
	{
		this.pwsh = pwsh;
		this.gitOptions = gitOptions;
		this.gitCloneConfiguration = gitCloneConfiguration;
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

	Task<PowerShellInvocationResult> IGitToolsInvoker.InvokeGitToolsAsync(string relativeScriptName, Action<PowerShell>? addParameters) =>
		pwsh.InvokeExternalScriptAsync(ToAbsoluteScriptPath(relativeScriptName), addParameters);

	Task<PowerShellInvocationResult> IGitToolsInvoker.InvokeGitToolsAsync<T>(string relativeScriptName, PSDataCollection<T> input, Action<PowerShell>? addParameters) =>
		pwsh.InvokeExternalScriptAsync(ToAbsoluteScriptPath(relativeScriptName), input, addParameters);

	private string ToAbsoluteScriptPath(string relativeScriptName)
	{
		return Path.Join(gitOptions.GitToolsDirectory, relativeScriptName);
	}

	public IPowerShellCommandInvoker PowerShellCommandInvoker { get; }

	public IGitToolsCommandInvoker GitToolsCommandInvoker { get; }

	IGitToolsInvoker IGitToolsCommandContext.GitToolsInvoker => this;

	IPowerShellInvoker IPowerShellCommandContext.PowerShellInvoker => pwsh;
}
