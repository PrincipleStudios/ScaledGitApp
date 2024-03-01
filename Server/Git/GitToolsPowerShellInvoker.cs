using Microsoft.Extensions.Options;
using PrincipleStudios.ScaledGitApp.Environment;
using PrincipleStudios.ScaledGitApp.ShellUtilities;
using System.Management.Automation;

namespace PrincipleStudios.ScaledGitApp.Git;

public sealed class GitToolsPowerShellInvoker : IGitToolsInvoker
{
	private readonly GitOptions gitOptions;
	private readonly Lazy<Task<Func<IPowerShell>>> powerShellFactory;
	private readonly ILogger<GitToolsPowerShellInvoker> logger;

	public GitToolsPowerShellInvoker(IOptions<GitOptions> options, PowerShellFactory psFactory, ILogger<GitToolsPowerShellInvoker> logger)
	{
		gitOptions = options.Value;
		powerShellFactory = new(() => CreatePowerShellWithGitDirectory(psFactory));
		this.logger = logger;
	}

	public GitToolsPowerShellInvoker(IOptions<GitOptions> options, Func<IPowerShell> psFactory, ILogger<GitToolsPowerShellInvoker> logger)
	{
		gitOptions = options.Value;
		powerShellFactory = new(Task.FromResult(psFactory));
		this.logger = logger;
	}

	async Task<Func<IPowerShell>> CreatePowerShellWithGitDirectory(PowerShellFactory psFactory)
	{
		using var ps = psFactory.Create();
		var absoluteInitialDirectory = Path.IsPathRooted(gitOptions.WorkingDirectory)
			? gitOptions.WorkingDirectory
			: Path.Join(Directory.GetCurrentDirectory(), gitOptions.WorkingDirectory);
		logger.UsingGitWorkingDirectory(absoluteInitialDirectory);

		// Creates if they do not exist already, recursively
		Directory.CreateDirectory(absoluteInitialDirectory);

		ps.SetCurrentWorkingDirectory(absoluteInitialDirectory);
		// Gets the _actual_ top level of the working directory, in case 
		var gitTopLevel = await ps.InvokeCliAsync("git", "rev-parse", "--show-toplevel") switch
		{
			{ HadErrors: false, Results: [PSObject item] } => item.ToString(),
			{ HadErrors: true } => absoluteInitialDirectory,
			_ => throw new InvalidOperationException("Unknown result from `git rev-parse --show-toplevel`")
		};

		return () =>
		{
			var result = psFactory.Create();
			result.SetCurrentWorkingDirectory(gitTopLevel);
			return result;
		};
	}

	private async Task<IPowerShell> CreateGitToolsPowershell()
	{
		return (await powerShellFactory.Value)();
	}

	// Can't use async if `T` is a generic of type Task, so we need to provide both
	// implementations. `await await` is theoretically quite efficient:
	// See https://stackoverflow.com/a/34832315/195653
	public async Task RunCommand(IGitToolsCommand<Task> command) =>
		await await RunCommandImplementation(command);
	public async Task<T> RunCommand<T>(IGitToolsCommand<Task<T>> command) =>
		await await RunCommandImplementation(command);

	private async Task<T> RunCommandImplementation<T>(IGitToolsCommand<T> command) where T : Task
	{
		using var pwsh = new GitToolsPowerShell(await CreateGitToolsPowershell(), gitOptions);
		using var activity = TracingHelper.StartActivity(command.GetType().Name);
		var result = command.RunCommand(pwsh);
		// Ensure the command has been awaited before disposing the activity, but since we can't await something of type T, this has no return type.
		await result.ConfigureAwait(false);
		return result;
	}
}
