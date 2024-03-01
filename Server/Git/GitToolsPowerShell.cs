using PrincipleStudios.ScaledGitApp.ShellUtilities;
using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace PrincipleStudios.ScaledGitApp.Git;

public record GitCloneConfiguration(string GitRootDirectory);

public sealed class GitToolsPowerShell : IGitToolsPowerShell, IDisposable
{
	private readonly IPowerShell pwsh;
	private readonly GitOptions gitOptions;

	public GitToolsPowerShell(IPowerShell pwsh, GitOptions gitOptions, GitCloneConfiguration gitCloneConfiguration)
	{
		this.pwsh = pwsh;
		this.gitOptions = gitOptions;
		this.pwsh.SetCurrentWorkingDirectory(gitCloneConfiguration.GitRootDirectory);
	}

	public void Dispose()
	{
		pwsh.Dispose();
	}

	// TODO: this should not get hard-coded
	public string UpstreamBranchName => "origin/_upstream";

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