using PrincipleStudios.ScaledGitApp.Commands;
using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.Git;

public sealed class PowerShellCommandContext(IPowerShellInvoker powerShell, ILogger logger, ICommandCache commandCache)
	: CachingCommandInvoker<IPowerShellCommandContext>(logger, commandCache), IPowerShellCommandContext
{
	public IPowerShellCommandInvoker PowerShellCommandInvoker => this;
	public IPowerShellInvoker PowerShellInvoker => powerShell;

	protected override Task<T> RunGenericCommand<T>(ICommand<T, IPowerShellCommandContext> command) =>
		RunGenericCommand(command, this);
}
