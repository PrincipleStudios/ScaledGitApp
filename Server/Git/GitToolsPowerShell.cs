using Microsoft.Extensions.Options;
using PrincipleStudios.ScaledGitApp.ShellUtilities;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace PrincipleStudios.ScaledGitApp.Git;

public sealed partial class GitToolsPowerShell : IGitToolsPowerShell
{
	private readonly GitOptions gitOptions;
	private readonly Lazy<Task<Func<IPowerShell>>> powerShellFactory;
	private readonly ILogger<GitToolsPowerShell> logger;
	private Runspace? runspace;
	private bool disposedValue;

	public GitToolsPowerShell(IOptions<GitOptions> options, PowerShellFactory psFactory, ILogger<GitToolsPowerShell> logger)
	{
		gitOptions = options.Value;
		powerShellFactory = new(() => CreatePowerShellWithGitDirectory(psFactory));
		this.logger = logger;
	}

	public GitToolsPowerShell(IOptions<GitOptions> options, Func<IPowerShell> psFactory, ILogger<GitToolsPowerShell> logger)
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

	private async Task<PowerShellInvocationResult> InvokeGitToolsAsync(string relativeScriptName, Action<PowerShell> addParameters)
	{
		using var ps = await CreateGitToolsPowershell();
		var scriptPath = Path.Join(gitOptions.GitToolsDirectory, relativeScriptName);

		return await ps.InvokeExternalScriptAsync(scriptPath, addParameters);
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
