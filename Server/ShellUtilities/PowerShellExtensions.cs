using PrincipleStudios.ScaledGitApp.Git;
using System.Management.Automation;

namespace PrincipleStudios.ScaledGitApp.ShellUtilities;

public static class PowerShellExtensions
{
	public static void SetCurrentWorkingDirectory(this PowerShell shell, string workingDirectory) =>
		shell.Runspace.SessionStateProxy.Path.SetLocation(workingDirectory);

	public static Task<PowerShellInvocationResult> InvokeCliAsync(this IPowerShell pwsh, string command, IEnumerable<string> arguments) =>
		pwsh.InvokeCliAsync(command, arguments.ToArray());
	public static IEnumerable<string> ToResultStrings(this PowerShellInvocationResult pwshResult)
	{
		ThrowIfHadErrors(pwshResult);
		return pwshResult.Results.Select(i => i.ToString());
	}

	public static void ThrowIfHadErrors(this PowerShellInvocationResult pwshResult)
	{
		if (pwshResult.HadErrors)
			throw GitException.From(pwshResult);
	}
}
