using PrincipleStudios.ScaledGitApp.ShellUtilities;
using System.Management.Automation;

namespace PrincipleStudios.ScaledGitApp.Git;

public interface IGitToolsPowerShellCommandContext : IGitToolsCommandInvoker, IPowerShellCliInvoker, IGitToolsInvoker
{
	string UpstreamBranchName { get; }
	string? ToLocalTrackingBranchName(string remoteBranchName);
}

public interface IPowerShellCommandContext : IPowerShellCommandInvoker, IPowerShellInvoker
{

}

public interface IGitToolsInvoker
{
	Task<PowerShellInvocationResult> InvokeGitToolsAsync(string relativeScriptName, Action<PowerShell>? addParameters = null);
	Task<PowerShellInvocationResult> InvokeGitToolsAsync<T>(string relativeScriptName, PSDataCollection<T> input, Action<PowerShell>? addParameters = null);
}