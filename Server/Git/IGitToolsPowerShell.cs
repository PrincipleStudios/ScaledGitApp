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
	Task<PowerShellInvocationResult> InvokeCliAsync(string command, params string[] arguments);
	Task<PowerShellInvocationResult> InvokeGitToolsAsync(string relativeScriptName, Action<PowerShell> addParameters);
}

public interface IGitToolsCommand<T> where T : Task
{
	T RunCommand(IGitToolsPowerShell pwsh);
}
