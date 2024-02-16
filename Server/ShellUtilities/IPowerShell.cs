using System.Management.Automation;

namespace PrincipleStudios.ScaledGitApp.ShellUtilities;

public interface IPowerShell : IDisposable
{
	void SetCurrentWorkingDirectory(string workingDirectory);

	Task<PowerShellInvocationResult> InvokeCliAsync(string command, params string[] arguments);
	Task<PowerShellInvocationResult> InvokeExternalScriptAsync(string externalScriptPath, Action<PowerShell>? addParameters = null);
}
