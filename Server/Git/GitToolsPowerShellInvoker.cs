using Microsoft.Extensions.Options;
using PrincipleStudios.ScaledGitApp.Environment;
using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.Git;

public sealed class GitToolsPowerShellInvoker : IGitToolsInvoker
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

	private async Task<GitCloneConfiguration> GetGitCloneConfiguration()
	{
		return await gitCloneConfigurationAccessor.Value;
	}

	// Can't use async if `T` is a generic of type Task, so we need to provide both
	// implementations. `await await` is theoretically quite efficient:
	// See https://stackoverflow.com/a/34832315/195653
	public async Task RunCommand(IGitToolsCommand<Task> command) =>
		await await GitToolsCommandImplementation(command);
	public async Task<T> RunCommand<T>(IGitToolsCommand<Task<T>> command) =>
		await await GitToolsCommandImplementation(command);
	public async Task RunCommand(IPowerShellCommand<Task> command) =>
		await await PowerShellCommandImplementation(command);
	public async Task<T> RunCommand<T>(IPowerShellCommand<Task<T>> command) =>
		await await PowerShellCommandImplementation(command);

	private async Task<T> GitToolsCommandImplementation<T>(IGitToolsCommand<T> command) where T : Task
	{
		var config = await GetGitCloneConfiguration();
		var pwsh = psFactory.Create();
		pwsh.SetCurrentWorkingDirectory(config.GitRootDirectory);
		using var gitToolsPwsh = new GitToolsPowerShell(pwsh, gitOptions, config);
		return await RunCommandImplementation(command, () => command.RunCommand(gitToolsPwsh));
	}

	private async Task<T> PowerShellCommandImplementation<T>(IPowerShellCommand<T> command) where T : Task
	{
		using var pwsh = psFactory.Create();
		pwsh.SetCurrentWorkingDirectory(gitOptions.WorkingDirectory);
		return await RunCommandImplementation(command, () => command.RunCommand(pwsh));
	}

	private async Task<T> RunCommandImplementation<T>(object command, Func<T> runCommand) where T : Task
	{
		using var activity = TracingHelper.StartActivity(command.GetType().Name);
		logger.RunningGitToolsPowerShellCommand(command.GetType().Name);
		return await EnsureComplete(runCommand);
	}

	private static async Task<T> EnsureComplete<T>(Func<T> target) where T : Task
	{
		var result = target();
		// Ensure the command has been awaited before disposing the activity, but since we can't await something of type T, this has no return type.
		await result.ConfigureAwait(false);
		return result;
	}
}
