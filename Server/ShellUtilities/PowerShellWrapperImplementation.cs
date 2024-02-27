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


	public void SetCurrentWorkingDirectory(string workingDirectory)
	{
		powerShell.SetCurrentWorkingDirectory(workingDirectory);
	}

	public async Task<PowerShellInvocationResult> InvokeCliAsync(string command, params string[] arguments)
	{
		using var activity = SetupCliActivity(command, arguments);
		return await powerShell.InvokeAsync().ToInvocationResult(powerShell);
	}

	public async Task<PowerShellInvocationResult> InvokeCliAsync<T>(string command, PSDataCollection<T> input, IEnumerable<string> arguments)
	{
		using var activity = SetupCliActivity(command, arguments);
		return await powerShell.InvokeAsync(input).ToInvocationResult(powerShell);
	}

	public async Task<PowerShellInvocationResult> InvokeExternalScriptAsync(string externalScriptPath, Action<PowerShell>? addParameters = null)
	{
		using var activity = SetupExternalScriptActivity(externalScriptPath, addParameters);
		return await powerShell.InvokeAsync().ToInvocationResult(powerShell);
	}

	public async Task<PowerShellInvocationResult> InvokeExternalScriptAsync<T>(string externalScriptPath, PSDataCollection<T> input, Action<PowerShell>? addParameters = null)
	{
		using var activity = SetupExternalScriptActivity(externalScriptPath, addParameters);
		return await powerShell.InvokeAsync(input).ToInvocationResult(powerShell);
	}

	private Activity? SetupExternalScriptActivity(string externalScriptPath, Action<PowerShell>? addParameters)
	{
		Activity? activity = TracingHelper.StartActivity(nameof(InvokeExternalScriptAsync));
		activity?.AddTag("script", externalScriptPath);

		powerShell.Commands.Clear();
		var externalCommand = powerShell.Runspace.SessionStateProxy.InvokeCommand.GetCommand(externalScriptPath, CommandTypes.ExternalScript);
		powerShell.AddCommand(externalCommand);
		addParameters?.Invoke(powerShell);
		return activity;
	}

	private Activity? SetupCliActivity(string command, IEnumerable<string> arguments)
	{
		Activity? activity = TracingHelper.StartActivity(nameof(InvokeCliAsync));
		activity?.AddTag("cmd", command);
		activity?.AddTag("cmd-args", string.Join(' ', arguments));

		powerShell.Commands.Clear();
		powerShell.AddCommand(command);
		foreach (var arg in arguments)
			powerShell.AddArgument(arg);
		return activity;
	}

	public void Dispose()
	{
		powerShell.Dispose();
	}
}
