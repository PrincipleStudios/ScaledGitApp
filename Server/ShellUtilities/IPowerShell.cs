using System.Management.Automation;

namespace PrincipleStudios.ScaledGitApp.ShellUtilities;

public interface IPowerShell : IPowerShellInvoker, IDisposable
{
	void SetCurrentWorkingDirectory(string workingDirectory);
}

public interface IPowerShellInvoker : IPowerShellCliInvoker
{
	Task<PowerShellInvocationResult> InvokeExternalScriptAsync(string externalScriptPath, Action<PowerShell>? addParameters = null);
	Task<PowerShellInvocationResult> InvokeExternalScriptAsync<T>(string externalScriptPath, PSDataCollection<T> input, Action<PowerShell>? addParameters = null);
}

public interface IPowerShellCliInvoker
{
	Task<PowerShellInvocationResult> InvokeCliAsync(string command, params string[] arguments);
	Task<PowerShellInvocationResult> InvokeCliAsync<T>(string command, PSDataCollection<T> input, IEnumerable<string> arguments);
}