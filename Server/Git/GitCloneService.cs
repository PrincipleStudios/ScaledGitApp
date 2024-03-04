using Microsoft.Extensions.Options;
using PrincipleStudios.ScaledGitApp.Environment;
using PrincipleStudios.ScaledGitApp.Git.ToolsCommands;
using System.Collections.Immutable;
using System.Diagnostics;

namespace PrincipleStudios.ScaledGitApp.Git;

public class GitCloneService : IHostedService
{
	private readonly GitOptions gitOptions;
	private readonly IGitToolsCommandInvoker gitToolsPowershell;
	private readonly ILogger<GitCloneService> logger;
	private readonly TaskCompletionSource<GitCloneConfiguration> detectedConfigurationTask = new();

	public Task<GitCloneConfiguration> DetectedConfigurationTask => detectedConfigurationTask.Task;

	public GitCloneService(IOptions<GitOptions> options, IGitToolsCommandInvoker gitToolsPowershell, ILogger<GitCloneService> logger)
	{
		gitOptions = options.Value;
		this.gitToolsPowershell = gitToolsPowershell;
		this.logger = logger;
#pragma warning disable CA1848 // Use the LoggerMessage delegates
		logger.LogCritical("Constructed");
#pragma warning restore CA1848 // Use the LoggerMessage delegates
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		// Spin out to background worker so as to not block the main app startup
		Task.Run(DetectCloneConfiguration, cancellationToken);
		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	public async Task DetectCloneConfiguration()
	{
		try
		{
			detectedConfigurationTask.SetResult(await LoadCloneConfiguration());
		}
		catch (Exception ex)
		{
			detectedConfigurationTask.SetException(ex);
			throw;
		}
	}

	private async Task<GitCloneConfiguration> LoadCloneConfiguration()
	{
		var absoluteInitialDirectory = Path.IsPathRooted(gitOptions.WorkingDirectory)
			? gitOptions.WorkingDirectory
			: Path.Join(Directory.GetCurrentDirectory(), gitOptions.WorkingDirectory);
		// Creates if the working directory does not exist already, recursively
		Directory.CreateDirectory(absoluteInitialDirectory);

		var (status, remotes) = await EnsureGitClone();

		if (status == GitCloneServiceStatus.CloneFailed)
			throw new InvalidOperationException("Could not clone repository");

		remotes ??= await gitToolsPowershell.RunCommand(new GitRemote());
		var gitTopLevel = await gitToolsPowershell.RunCommand(new ResolveTopLevelDirectory(absoluteInitialDirectory));

		var configuration = await gitToolsPowershell.RunCommand(new GitConfigurationList());
		var scaledGitRemote = SafeGetConfiguration("scaled-git.remote").SingleOrDefault()
			?? remotes.Remotes[0].Alias;
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

	public async Task<(GitCloneServiceStatus Status, GitRemoteResult? Remotes)> EnsureGitClone()
	{
		using Activity? activity = TracingHelper.StartActivity(nameof(EnsureGitClone));

		if (gitOptions.Repository == null)
		{
			logger.NoGitRepositoryConfigured();
			return (GitCloneServiceStatus.NoRepository, null);
		}

		var result = await LoadRemotes();
		if (result is (GitCloneServiceStatus Status, GitRemoteResult Remotes) tuple) return tuple;
		return (await RunNewClone(gitOptions.Repository), null);
	}

	private async Task<(GitCloneServiceStatus Status, GitRemoteResult Remotes)?> LoadRemotes()
	{
		try
		{
			var remotes = await gitToolsPowershell.RunCommand(new GitRemote());

			if (remotes.Remotes.Count == 0)
			{
				logger.GitWithNoRemotes();
				return (GitCloneServiceStatus.NoRemotes, remotes);
			}
			if (remotes.Remotes.Count > 1)
			{
				logger.MultipleGitRepositoriesConfigured(remotes.Remotes.Select(r => r.Alias));
				return (GitCloneServiceStatus.MultipleRemotes, remotes);
			}

			if (remotes.Remotes[0].FetchUrl != gitOptions.Repository)
			{
				logger.GitRepositoryMismatch(expected: gitOptions.Repository, actual: remotes.Remotes[0].FetchUrl);
				return (GitCloneServiceStatus.RepositoryMismatch, remotes);
			}

			logger.GitAlreadyCloned(remote: gitOptions.Repository, directory: gitOptions.WorkingDirectory);
			return (GitCloneServiceStatus.AlreadyCloned, remotes);
		}
		catch (GitException)
		{
			// Exception encountered when listing remotes; hopefully is not a git directory
			return null;
		}
		catch (Exception ex)
		{
			logger.GitFailedToCloneWithUnknownException(ex);
			throw;
		}
	}

	private async Task<GitCloneServiceStatus> RunNewClone(string repository)
	{
		try
		{
			await gitToolsPowershell.RunCommand(new GitClone(repository));
			logger.GitClonedSuccessfully(repository, gitOptions.WorkingDirectory);
			return GitCloneServiceStatus.ClonedSuccessfully;
		}
		catch (GitException ex)
		{
			logger.GitFailedToClone(ex);
			return GitCloneServiceStatus.CloneFailed;
		}
		catch (Exception ex)
		{
			logger.GitFailedToCloneWithUnknownException(ex);
			throw;
		}
	}
}

public enum GitCloneServiceStatus
{
	NoRepository,
	NoRemotes,
	MultipleRemotes,
	RepositoryMismatch,
	AlreadyCloned,
	ClonedSuccessfully,
	CloneFailed,
}