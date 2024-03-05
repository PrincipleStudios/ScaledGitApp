using PrincipleStudios.ScaledGitApp.ShellUtilities;
using System.Management.Automation;

namespace PrincipleStudios.ScaledGitApp.Git;

public sealed class GitToolsInvoker(IPowerShellInvoker pwsh, GitOptions gitOptions) : IGitToolsInvoker
{

	Task<PowerShellInvocationResult> IGitToolsInvoker.InvokeGitToolsAsync(string relativeScriptName, Action<PowerShell>? addParameters) =>
		pwsh.InvokeExternalScriptAsync(ToAbsoluteScriptPath(relativeScriptName), addParameters);

	Task<PowerShellInvocationResult> IGitToolsInvoker.InvokeGitToolsAsync<T>(string relativeScriptName, PSDataCollection<T> input, Action<PowerShell>? addParameters) =>
		pwsh.InvokeExternalScriptAsync(ToAbsoluteScriptPath(relativeScriptName), input, addParameters);

	private string ToAbsoluteScriptPath(string relativeScriptName) =>
		Path.Join(gitOptions.GitToolsDirectory, relativeScriptName);
}