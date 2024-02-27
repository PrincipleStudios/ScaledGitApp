using PrincipleStudios.ScaledGitApp.ShellUtilities;
using System.Management.Automation;

namespace PrincipleStudios.ScaledGitApp.Git;

public interface IGitToolsInvoker
{
	Task RunCommand(IGitToolsCommand<Task> command);
	Task<T> RunCommand<T>(IGitToolsCommand<Task<T>> command);
}

public interface IGitToolsPowerShell
{
	string UpstreamBranchName { get; }

	Task<PowerShellInvocationResult> InvokeCliAsync(string command, params string[] arguments);
	Task<PowerShellInvocationResult> InvokeCliAsync<T>(string command, IEnumerable<string> arguments, PSDataCollection<T> input);
	Task<PowerShellInvocationResult> InvokeGitToolsAsync(string relativeScriptName, Action<PowerShell> addParameters);
	Task<PowerShellInvocationResult> InvokeGitToolsAsync<T>(string relativeScriptName, Action<PowerShell> addParameters, PSDataCollection<T> input);
}

public interface IGitToolsCommand<T> where T : Task
{
	T RunCommand(IGitToolsPowerShell pwsh);
}
