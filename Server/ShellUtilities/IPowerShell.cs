using System.Management.Automation;

namespace PrincipleStudios.ScaledGitApp.ShellUtilities;

public interface IPowerShell : IPowerShellInvoker, IDisposable
{
}

public interface IPowerShellInvoker : IPowerShellCliInvoker
{
	void SetCurrentWorkingDirectory(string workingDirectory);
	Task<PowerShellInvocationResult> InvokeExternalScriptAsync(string externalScriptPath, Action<PowerShell>? addParameters = null);
	Task<PowerShellInvocationResult> InvokeExternalScriptAsync<T>(string externalScriptPath, PSDataCollection<T> input, Action<PowerShell>? addParameters = null);
}

public interface IPowerShellCliInvoker
{
	Task<PowerShellInvocationResult> InvokeCliAsync(string command, params string[] arguments);
	Task<PowerShellInvocationResult> InvokeCliAsync<T>(string command, PSDataCollection<T> input, IEnumerable<string> arguments);
}