using Microsoft.Extensions.Options;
using PrincipleStudios.ScaledGitApp.Commands;
using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.Git;

public sealed class GitToolsCommandInvoker(
	IOptions<GitOptions> options,
	PowerShellFactory psFactory,
	Lazy<Task<GitCloneConfiguration>> gitCloneConfigurationAccessor,
	ILogger<GitToolsCommandInvoker> logger,
	ICommandCache commandCache
) : CachingCommandInvoker<IGitToolsCommandContext>(logger, commandCache)
{
	public GitToolsCommandInvoker(IOptions<GitOptions> options, PowerShellFactory psFactory, Func<Task<GitCloneConfiguration>> gitCloneConfiguration, ILogger<GitToolsCommandInvoker> logger, ICommandCache commandCache)
		: this(options, psFactory, new Lazy<Task<GitCloneConfiguration>>(gitCloneConfiguration), logger, commandCache)
	{
	}

	public GitToolsCommandInvoker(IOptions<GitOptions> options, PowerShellFactory psFactory, GitCloneConfiguration gitCloneConfiguration, ILogger<GitToolsCommandInvoker> logger, ICommandCache commandCache)
		: this(options, psFactory, () => Task.FromResult(gitCloneConfiguration), logger, commandCache)
	{
	}

	protected override async Task<T> RunGenericCommand<T>(ICommand<T, IGitToolsCommandContext> command)
	{
		using var pwsh = psFactory.Create();
		var config = await gitCloneConfigurationAccessor.Value;
		pwsh.SetCurrentWorkingDirectory(config.GitRootDirectory);
		var context = new GitToolsCommandContext(pwsh, new GitToolsInvoker(pwsh, options.Value), config, logger, Cache);
		return await RunGenericCommand(command, context);
	}
}
