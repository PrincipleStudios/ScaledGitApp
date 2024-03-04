using PrincipleStudios.ScaledGitApp.ShellUtilities;
using System.Management.Automation;

namespace PrincipleStudios.ScaledGitApp.Git;

public sealed class GitToolsPowerShell : IGitToolsPowerShell, IDisposable
{
	private readonly IPowerShell pwsh;
	private readonly GitOptions gitOptions;
	private readonly GitCloneConfiguration gitCloneConfiguration;

	public GitToolsPowerShell(IPowerShell pwsh, GitOptions gitOptions, GitCloneConfiguration gitCloneConfiguration)
	{
		this.pwsh = pwsh;
		this.gitOptions = gitOptions;
		this.gitCloneConfiguration = gitCloneConfiguration;
		this.pwsh.SetCurrentWorkingDirectory(gitCloneConfiguration.GitRootDirectory);
	}

	public void Dispose()
	{
		pwsh.Dispose();
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

	public Task<PowerShellInvocationResult> InvokeCliAsync(string command, params string[] arguments) =>
		pwsh.InvokeCliAsync(command, arguments);

	public Task<PowerShellInvocationResult> InvokeCliAsync<T>(string command, IEnumerable<string> arguments, PSDataCollection<T> input) =>
		pwsh.InvokeCliAsync(command, input, arguments);

	public Task<PowerShellInvocationResult> InvokeGitToolsAsync(string relativeScriptName, Action<PowerShell> addParameters) =>
		pwsh.InvokeExternalScriptAsync(ToAbsoluteScriptPath(relativeScriptName), addParameters);

	public Task<PowerShellInvocationResult> InvokeGitToolsAsync<T>(string relativeScriptName, Action<PowerShell> addParameters, PSDataCollection<T> input) =>
		pwsh.InvokeExternalScriptAsync(ToAbsoluteScriptPath(relativeScriptName), input, addParameters);

	private string ToAbsoluteScriptPath(string relativeScriptName)
	{
		return Path.Join(gitOptions.GitToolsDirectory, relativeScriptName);
	}
}