using PrincipleStudios.ScaledGitApp.Git;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace PrincipleStudios.ScaledGitApp.ShellUtililties;

public static class PowerShellExtensions
{
	public static void SetCurrentWorkingDirectory(this PowerShell shell, string workingDirectory) =>
		shell.Runspace.SetCurrentWorkingDirectory(workingDirectory);
	public static void SetCurrentWorkingDirectory(this Runspace runspace, string workingDirectory) =>
		runspace.SessionStateProxy.Path.SetLocation(workingDirectory);

	public static Task<PowerShellInvocationResult> InvokeCliAsync(this PowerShell powerShell, string command, params string[] arguments) =>
		InvokeCliAsync(powerShell, command, (IEnumerable<string>)arguments);
	public static async Task<PowerShellInvocationResult> InvokeCliAsync(this PowerShell powerShell, string command, IEnumerable<string> arguments)
	{
		powerShell.Commands.Clear();

		powerShell.AddCommand(command);
		foreach (var arg in arguments)
			powerShell.AddArgument(arg);
		return await PowerShellInvocationResult.InvokeAsync(powerShell);
	}

	public static async Task<PowerShellInvocationResult> InvokeExternalScriptAsync(this PowerShell powerShell, string externalScriptPath, Action<PowerShell>? addParameters = null)
	{
		powerShell.Commands.Clear();
		var externalCommand = powerShell.Runspace.SessionStateProxy.InvokeCommand.GetCommand(externalScriptPath, CommandTypes.ExternalScript);
		powerShell.AddCommand(externalCommand);
		addParameters?.Invoke(powerShell);

		return await PowerShellInvocationResult.InvokeAsync(powerShell);
	}
}
