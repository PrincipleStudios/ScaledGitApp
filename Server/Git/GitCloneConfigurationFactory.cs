using Microsoft.Extensions.Options;
using PrincipleStudios.ScaledGitApp.ShellUtilities;
using System.Collections.Immutable;
using System.Management.Automation;

namespace PrincipleStudios.ScaledGitApp.Git;

public sealed class GitCloneConfigurationFactory(PowerShellFactory PsFactory, IOptions<GitOptions> Options, ILogger<GitCloneConfigurationFactory> Logger)
{
	public async Task<GitCloneConfiguration> DetectGitCloneConfiguration()
	{
		using var ps = PsFactory.Create();
		var gitTopLevel = await GetRootDirectory(ps);

		var configuration = await GetConfiguration(ps);
		var scaledGitRemote = SafeGetConfiguration("scaled-git.remote").SingleOrDefault()
			?? (await GetAllRemotes(ps)).First();
		var upstreamBranchName = SafeGetConfiguration("scaled-git.upstreamBranch").DefaultIfEmpty("_upstream").Single();
		var fetchMapping = SafeGetConfiguration($"remote.{scaledGitRemote}.fetch").Select(FetchMapping.Parse).ToImmutableArray();

		return new GitCloneConfiguration(
			GitRootDirectory: gitTopLevel,
			RemoteName: scaledGitRemote,
			UpstreamBranchName: upstreamBranchName,
			FetchMapping: fetchMapping
		);

		IEnumerable<string> SafeGetConfiguration(string key)
		{
			return configuration.TryGetValue(key.ToLower(), out var remoteEntries) ? remoteEntries : Enumerable.Empty<string>();
		}
	}

	public async Task<string> GetRootDirectory(IPowerShell ps)
	{
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
		return gitTopLevel;
	}

	public async Task<IEnumerable<string>> GetAllRemotes(IPowerShell ps)
	{
		var remoteResult = await ps.InvokeCliAsync("git", "remote");
		remoteResult.ThrowIfHadErrors();
		var allRemotes = remoteResult.ToResultStrings();
		return allRemotes;
	}

	public async Task<Dictionary<string, IEnumerable<string>>> GetConfiguration(IPowerShell ps)
	{
		var configurationResult = await ps.InvokeCliAsync("git", "config", "--list");
		configurationResult.ThrowIfHadErrors();
		return
			(
				from configurationLine in configurationResult.ToResultStrings()
				let parts = configurationLine.Split('=', 2)
				group parts[1] by parts[0]
			).ToDictionary(g => g.Key, g => g as IEnumerable<string>);
	}
}