using PrincipleStudios.ScaledGitApp.Commands;
using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.Git;

public sealed class GitToolsCommandContext : CommandInvoker<IGitToolsCommandContext>, IGitToolsCommandContext
{
	private readonly IPowerShellInvoker pwsh;

	public GitToolsCommandContext(IPowerShellInvoker pwsh, IGitToolsInvoker gitToolsInvoker, IGitCloneConfiguration gitCloneConfiguration, ILogger logger)
		: base(logger)
	{
		this.pwsh = pwsh;
		GitCloneConfiguration = gitCloneConfiguration;
		GitToolsInvoker = gitToolsInvoker;
	}

	public IPowerShellCommandInvoker PowerShellCommandInvoker => this;

	public IGitToolsCommandInvoker GitToolsCommandInvoker => this;

	public IGitToolsInvoker GitToolsInvoker { get; }

	public IGitCloneConfiguration GitCloneConfiguration { get; }

	IPowerShellInvoker IPowerShellCommandContext.PowerShellInvoker => pwsh;

	protected override Task<T> RunGenericCommand<T>(ICommand<T, IGitToolsCommandContext> command) =>
		RunGenericCommand(command, this);
}
