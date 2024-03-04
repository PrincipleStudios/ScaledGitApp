using PrincipleStudios.ScaledGitApp.Environment;
using PrincipleStudios.ScaledGitApp.ShellUtilities;
using System.Management.Automation;

namespace PrincipleStudios.ScaledGitApp.Git;

public sealed class PowerShellCommandContext(IPowerShellInvoker powerShell, ILogger logger) : IPowerShellCommandContext
{
	public Task<PowerShellInvocationResult> InvokeCliAsync(string command, params string[] arguments) =>
		powerShell.InvokeCliAsync(command, arguments);

	public Task<PowerShellInvocationResult> InvokeCliAsync<T>(string command, PSDataCollection<T> input, IEnumerable<string> arguments) =>
		powerShell.InvokeCliAsync(command, input, arguments);

	public async Task RunCommand(IPowerShellCommand<Task> command) =>
		await await RunGenericCommand(command);
	public async Task<T> RunCommand<T>(IPowerShellCommand<Task<T>> command) =>
		await await RunGenericCommand(command);

	public void SetCurrentWorkingDirectory(string workingDirectory) =>
		powerShell.SetCurrentWorkingDirectory(workingDirectory);
	public Task<PowerShellInvocationResult> InvokeExternalScriptAsync(string externalScriptPath, Action<PowerShell>? addParameters = null) =>
		powerShell.InvokeExternalScriptAsync(externalScriptPath, addParameters);
	public Task<PowerShellInvocationResult> InvokeExternalScriptAsync<T>(string externalScriptPath, PSDataCollection<T> input, Action<PowerShell>? addParameters = null) =>
		powerShell.InvokeExternalScriptAsync(externalScriptPath, input, addParameters);


	private async Task<T> RunGenericCommand<T>(IPowerShellCommand<T> command) where T : Task =>
		await CommandContextInvoker.RunCommand(command.RunCommand, this, logger);
}