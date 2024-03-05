using PrincipleStudios.ScaledGitApp.Commands;
using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.Git;

public sealed class GitToolsCommandContext : IGitToolsCommandContext
{
	private readonly IPowerShellInvoker pwsh;

	public GitToolsCommandContext(IPowerShellInvoker pwsh, IGitToolsInvoker gitToolsInvoker, IGitCloneConfiguration gitCloneConfiguration, ILogger logger)
	{
		this.pwsh = pwsh;
		GitCloneConfiguration = gitCloneConfiguration;
		GitToolsInvoker = gitToolsInvoker;
		PowerShellCommandInvoker = new InstanceCommandInvoker<IPowerShellCommandContext>(this, logger);
		GitToolsCommandInvoker = new InstanceCommandInvoker<IGitToolsCommandContext>(this, logger);
	}

	public IPowerShellCommandInvoker PowerShellCommandInvoker { get; }

	public IGitToolsCommandInvoker GitToolsCommandInvoker { get; }

	public IGitToolsInvoker GitToolsInvoker { get; }

	public IGitCloneConfiguration GitCloneConfiguration { get; }

	IPowerShellInvoker IPowerShellCommandContext.PowerShellInvoker => pwsh;
}
