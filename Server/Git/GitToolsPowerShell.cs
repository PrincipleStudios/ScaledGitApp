using Microsoft.Extensions.Options;
using PrincipleStudios.ScaledGitApp.ShellUtilities;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace PrincipleStudios.ScaledGitApp.Git;

public sealed class GitToolsPowerShellInvoker : IGitToolsInvoker
{
	private readonly GitOptions gitOptions;
	private readonly Lazy<Task<Func<IPowerShell>>> powerShellFactory;
	private readonly ILogger<GitToolsPowerShellInvoker> logger;
	private Runspace? runspace;
	private bool disposedValue;

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

		runspace = psFactory.CreateRunspace();
		runspace.SetCurrentWorkingDirectory(gitTopLevel);
		return () => psFactory.Create(runspace);
	}

	private async Task<IPowerShell> CreateGitToolsPowershell()
	{
		return (await powerShellFactory.Value)();
	}

	public async Task RunCommand(IGitToolsCommand<Task> command)
	{
		using var pwsh = new GitToolsPowerShell(await CreateGitToolsPowershell(), gitOptions);
		await command.RunCommand(pwsh);
	}
	public async Task<T> RunCommand<T>(IGitToolsCommand<Task<T>> command)
	{
		using var pwsh = new GitToolsPowerShell(await CreateGitToolsPowershell(), gitOptions);
		return await command.RunCommand(pwsh);
	}

	#region Dispose Pattern
	private void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				runspace?.Dispose();
			}

			runspace = null;
			disposedValue = true;
		}
	}

	public void Dispose()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	#endregion Dispose Pattern
}

public sealed class GitToolsPowerShell : IGitToolsPowerShell, IDisposable
{
	private readonly IPowerShell pwsh;
	private readonly GitOptions gitOptions;

	public GitToolsPowerShell(IPowerShell pwsh, GitOptions gitOptions)
	{
		this.pwsh = pwsh;
		this.gitOptions = gitOptions;
	}

	public void Dispose()
	{
		pwsh.Dispose();
	}

	public async Task<PowerShellInvocationResult> InvokeCliAsync(string command, params string[] arguments)
	{
		return await pwsh.InvokeCliAsync($"{command}", arguments);
	}

	public async Task<PowerShellInvocationResult> InvokeGitToolsAsync(string relativeScriptName, Action<PowerShell> addParameters)
	{
		var scriptPath = Path.Join(gitOptions.GitToolsDirectory, relativeScriptName);

		return await pwsh.InvokeExternalScriptAsync(scriptPath, addParameters);
	}

}