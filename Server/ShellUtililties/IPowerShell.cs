using System.Management.Automation;

namespace PrincipleStudios.ScaledGitApp.ShellUtililties;

public interface IPowerShell : IDisposable
{
	void SetCurrentWorkingDirectory(string workingDirectory);
	Task<PowerShellInvocationResult> InvokeCliAsync(string command, params string[] arguments) =>
		InvokeCliAsync(command, (IEnumerable<string>)arguments);
	Task<PowerShellInvocationResult> InvokeCliAsync(string command, IEnumerable<string> arguments);
	Task<PowerShellInvocationResult> InvokeExternalScriptAsync(string externalScriptPath, Action<PowerShell>? addParameters = null);
}
