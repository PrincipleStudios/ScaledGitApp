using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.Git;

public static class UsabilityExtensions
{
	public static Task<PowerShellInvocationResult> InvokeCliAsync(this IGitToolsPowerShell pwsh, string command, IEnumerable<string> arguments) =>
		pwsh.InvokeCliAsync(command, arguments.ToArray());
}
