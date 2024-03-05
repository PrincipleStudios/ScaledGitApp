using Microsoft.Extensions.Options;
using PrincipleStudios.ScaledGitApp.Commands;
using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.Git;

public sealed class GitToolsPowerShellInvoker
{
	private readonly GitOptions gitOptions;
	private readonly Lazy<Task<GitCloneConfiguration>> gitCloneConfigurationAccessor;
	private readonly PowerShellFactory psFactory;
	private readonly ILogger<GitToolsPowerShellInvoker> logger;

	public GitToolsPowerShellInvoker(IOptions<GitOptions> options, PowerShellFactory psFactory, Func<Task<GitCloneConfiguration>> gitCloneConfiguration, ILogger<GitToolsPowerShellInvoker> logger)
	{
		gitOptions = options.Value;
		gitCloneConfigurationAccessor = new(gitCloneConfiguration);
		this.psFactory = psFactory;
		this.logger = logger;

		GitToolsCommandInvoker = new DisposableContextCommandInvoker<IGitToolsCommandContext>(BuildGitToolsPowerShellContext, logger);
		PowerShellCommandInvoker = new DisposableContextCommandInvoker<IPowerShellCommandContext>(BuildPowerShellContext, logger);
	}

	public GitToolsPowerShellInvoker(IOptions<GitOptions> options, PowerShellFactory psFactory, GitCloneConfiguration gitCloneConfiguration, ILogger<GitToolsPowerShellInvoker> logger)
		: this(options, psFactory, () => Task.FromResult(gitCloneConfiguration), logger)
	{
	}

	public IGitToolsCommandInvoker GitToolsCommandInvoker { get; }
	public IPowerShellCommandInvoker PowerShellCommandInvoker { get; }

	private async Task<GitCloneConfiguration> GetGitCloneConfiguration()
	{
		return await gitCloneConfigurationAccessor.Value;
	}

	private async Task<DisposableContext<IGitToolsCommandContext>> BuildGitToolsPowerShellContext()
	{
		var pwsh = psFactory.Create();
		var config = await GetGitCloneConfiguration();
		pwsh.SetCurrentWorkingDirectory(config.GitRootDirectory);
		var gitToolsPwsh = new GitToolsCommandContext(pwsh, gitOptions, config, logger);
		return new(gitToolsPwsh, pwsh);
	}

	private Task<DisposableContext<IPowerShellCommandContext>> BuildPowerShellContext()
	{
		var pwsh = psFactory.Create();
		pwsh.SetCurrentWorkingDirectory(gitOptions.WorkingDirectory);
		var gitToolsPwsh = new PowerShellCommandContext(pwsh, logger);
		return Task.FromResult(new DisposableContext<IPowerShellCommandContext>(gitToolsPwsh, pwsh));
	}
}
