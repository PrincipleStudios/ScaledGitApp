using PrincipleStudios.ScaledGitApp.Commands;
using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.Git;

public sealed class GitToolsCommandContext(
	IPowerShellInvoker pwsh,
	IGitToolsInvoker gitToolsInvoker,
	IGitCloneConfiguration gitCloneConfiguration,
	ILogger logger,
	ICommandCache commandCache
) : CachingCommandInvoker<IGitToolsCommandContext>(logger, commandCache), IGitToolsCommandContext
{
	public IPowerShellCommandInvoker PowerShellCommandInvoker => this;

	public IGitToolsCommandInvoker GitToolsCommandInvoker => this;

	public IGitToolsInvoker GitToolsInvoker => gitToolsInvoker;

	public IGitCloneConfiguration GitCloneConfiguration => gitCloneConfiguration;

	IPowerShellInvoker IPowerShellCommandContext.PowerShellInvoker => pwsh;

	protected override Task<T> RunGenericCommand<T>(ICommand<T, IGitToolsCommandContext> command) =>
		RunGenericCommand(command, this);
}
