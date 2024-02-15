using PrincipleStudios.ScaledGitApp.Environment;
using System.Diagnostics;
using System.Management.Automation;

namespace PrincipleStudios.ScaledGitApp.ShellUtilities;

internal sealed class PowerShellWrapperImplementation : IPowerShell
{
	private readonly PowerShell powerShell;

	public PowerShellWrapperImplementation(PowerShell powerShell)
	{
		this.powerShell = powerShell;
	}

	public async Task<PowerShellInvocationResult> InvokeCliAsync(string command, IEnumerable<string> arguments)
	{
		using Activity? activity = TracingHelper.StartActivity(nameof(InvokeCliAsync));
		activity?.AddTag("cmd", command);
		activity?.AddTag("cmd-args", string.Join(' ', arguments));
		powerShell.Commands.Clear();

		powerShell.AddCommand(command);
		foreach (var arg in arguments)
			powerShell.AddArgument(arg);
		return await PowerShellInvocationResult.InvokeAsync(powerShell);
	}

	public void SetCurrentWorkingDirectory(string workingDirectory)
	{
		powerShell.SetCurrentWorkingDirectory(workingDirectory);
	}

	public async Task<PowerShellInvocationResult> InvokeExternalScriptAsync(string externalScriptPath, Action<PowerShell>? addParameters = null)
	{
		using Activity? activity = TracingHelper.StartActivity(nameof(InvokeExternalScriptAsync));
		activity?.AddTag("script", externalScriptPath);

		powerShell.Commands.Clear();
		var externalCommand = powerShell.Runspace.SessionStateProxy.InvokeCommand.GetCommand(externalScriptPath, CommandTypes.ExternalScript);
		powerShell.AddCommand(externalCommand);
		addParameters?.Invoke(powerShell);

		return await PowerShellInvocationResult.InvokeAsync(powerShell);
	}

	public void Dispose()
	{
		powerShell.Dispose();
	}
}
