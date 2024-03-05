using Microsoft.Extensions.Options;
using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.Git;

public sealed class GitToolsPowerShellInvoker : IGitToolsCommandInvoker, IPowerShellCommandInvoker
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
	}

	public GitToolsPowerShellInvoker(IOptions<GitOptions> options, PowerShellFactory psFactory, GitCloneConfiguration gitCloneConfiguration, ILogger<GitToolsPowerShellInvoker> logger)
		: this(options, psFactory, () => Task.FromResult(gitCloneConfiguration), logger)
	{
	}

	IPowerShellCommandInvoker IGitToolsCommandInvoker.PowerShellCommandInvoker => this;

	private async Task<GitCloneConfiguration> GetGitCloneConfiguration()
	{
		return await gitCloneConfigurationAccessor.Value;
	}

	public async Task RunCommand(IGitToolsCommand<Task> command)
	{
		using var pwsh = psFactory.Create();
		var gitToolsPwsh = await BuildGitToolsPowerShellContext(pwsh);
		await gitToolsPwsh.RunCommand(command);
	}
	public async Task<T> RunCommand<T>(IGitToolsCommand<Task<T>> command)
	{
		using var pwsh = psFactory.Create();
		var gitToolsPwsh = await BuildGitToolsPowerShellContext(pwsh);
		return await gitToolsPwsh.RunCommand(command);
	}
	public async Task RunCommand(IPowerShellCommand<Task> command)
	{
		using var pwsh = psFactory.Create();
		await BuildPowerShellContext(pwsh).RunCommand(command);
	}
	public async Task<T> RunCommand<T>(IPowerShellCommand<Task<T>> command)
	{
		using var pwsh = psFactory.Create();
		return await BuildPowerShellContext(pwsh).RunCommand(command);
	}

	private async Task<GitToolsPowerShellCommandContext> BuildGitToolsPowerShellContext(IPowerShellInvoker pwsh)
	{
		var config = await GetGitCloneConfiguration();
		pwsh.SetCurrentWorkingDirectory(config.GitRootDirectory);
		var gitToolsPwsh = new GitToolsPowerShellCommandContext(pwsh, gitOptions, config, logger);
		return gitToolsPwsh;
	}

	private PowerShellCommandContext BuildPowerShellContext(IPowerShellInvoker pwsh)
	{
		pwsh.SetCurrentWorkingDirectory(gitOptions.WorkingDirectory);
		var gitToolsPwsh = new PowerShellCommandContext(pwsh, logger);
		return gitToolsPwsh;
	}
}
