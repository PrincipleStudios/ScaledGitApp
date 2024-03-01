using Microsoft.Extensions.Options;
using PrincipleStudios.ScaledGitApp.ShellUtilities;
using System.Management.Automation;

namespace PrincipleStudios.ScaledGitApp.Git;

public sealed class GitCloneConfigurationFactory(PowerShellFactory PsFactory, IOptions<GitOptions> Options, ILogger<GitCloneConfigurationFactory> Logger)
{
	public async Task<GitCloneConfiguration> DetectGitCloneConfiguration()
	{
		using var ps = PsFactory.Create();
		var absoluteInitialDirectory = Path.IsPathRooted(Options.Value.WorkingDirectory)
			? Options.Value.WorkingDirectory
			: Path.Join(Directory.GetCurrentDirectory(), Options.Value.WorkingDirectory);
		Logger.UsingGitWorkingDirectory(absoluteInitialDirectory);

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

		return new GitCloneConfiguration(
			GitRootDirectory: gitTopLevel
		);
	}

}