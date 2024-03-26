using PrincipleStudios.ScaledGitApp.ShellUtilities;
using System.Management.Automation;

namespace PrincipleStudios.ScaledGitApp.Git;

internal static class UsabilityExtensions
{
	public static Task<PowerShellInvocationResult> InvokeGitToolsAsync(this IGitToolsCommandContext target, string relativeScriptName, Action<PowerShell>? addParameters = null) =>
		target.GitToolsInvoker.InvokeGitToolsAsync(relativeScriptName, addParameters);
	public static Task<PowerShellInvocationResult> InvokeGitToolsAsync<T>(this IGitToolsCommandContext target, string relativeScriptName, PSDataCollection<T> input, Action<PowerShell>? addParameters = null) =>
		target.GitToolsInvoker.InvokeGitToolsAsync(relativeScriptName, input, addParameters);

	public static Task RunCommand(this IGitToolsCommandContext target, IGitToolsCommand<Task> command) =>
		target.GitToolsCommandInvoker.RunCommand(command);
	public static Task<T> RunCommand<T>(this IGitToolsCommandContext target, IGitToolsCommand<Task<T>> command) =>
		target.GitToolsCommandInvoker.RunCommand(command);

	public static Task RunCommand(this IPowerShellCommandContext target, IPowerShellCommand<Task> command) =>
		target.PowerShellCommandInvoker.RunCommand(command);
	public static Task<T> RunCommand<T>(this IPowerShellCommandContext target, IPowerShellCommand<Task<T>> command) =>
		target.PowerShellCommandInvoker.RunCommand(command);

	public static Task<PowerShellInvocationResult> InvokeExternalScriptAsync(this IPowerShellCommandContext target, string externalScriptPath, Action<PowerShell>? addParameters = null) =>
		target.PowerShellInvoker.InvokeExternalScriptAsync(externalScriptPath, addParameters);
	public static Task<PowerShellInvocationResult> InvokeExternalScriptAsync<T>(this IPowerShellCommandContext target, string externalScriptPath, PSDataCollection<T> input, Action<PowerShell>? addParameters = null) =>
		target.PowerShellInvoker.InvokeExternalScriptAsync(externalScriptPath, input, addParameters);

	public static Task<PowerShellInvocationResult> InvokeCliAsync(this IPowerShellCommandContext target, string command, params string[] arguments) =>
		target.PowerShellInvoker.InvokeCliAsync(command, arguments);
	public static Task<PowerShellInvocationResult> InvokeCliAsync<T>(this IPowerShellCommandContext target, string command, PSDataCollection<T> input, IEnumerable<string> arguments) =>
		target.PowerShellInvoker.InvokeCliAsync(command, input, arguments);

}