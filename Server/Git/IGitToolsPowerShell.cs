using PrincipleStudios.ScaledGitApp.ShellUtilities;
using System.Management.Automation;

namespace PrincipleStudios.ScaledGitApp.Git;

public interface IGitToolsPowerShell
{
	string UpstreamBranchName { get; }
	string? ToLocalTrackingBranchName(string remoteBranchName);

	Task<PowerShellInvocationResult> InvokeCliAsync(string command, params string[] arguments);
	Task<PowerShellInvocationResult> InvokeCliAsync<T>(string command, IEnumerable<string> arguments, PSDataCollection<T> input);
	Task<PowerShellInvocationResult> InvokeGitToolsAsync(string relativeScriptName, Action<PowerShell> addParameters);
	Task<PowerShellInvocationResult> InvokeGitToolsAsync<T>(string relativeScriptName, Action<PowerShell> addParameters, PSDataCollection<T> input);
}
