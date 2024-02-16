using Microsoft.Extensions.Options;
using PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

namespace PrincipleStudios.ScaledGitApp.Git;

public class GitCloneService : IHostedService
{
	private readonly GitOptions gitOptions;
	private readonly IGitToolsInvoker gitToolsPowershell;
	private readonly ILogger<GitCloneService> logger;

	public GitCloneService(IOptions<GitOptions> options, IGitToolsInvoker gitToolsPowershell, ILogger<GitCloneService> logger)
	{
		gitOptions = options.Value;
		this.gitToolsPowershell = gitToolsPowershell;
		this.logger = logger;
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		// Spin out to background worker so as to not block the main app startup
		Task.Run(EnsureGitClone, cancellationToken);
		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	public async Task<GitCloneServiceStatus> EnsureGitClone()
	{
		if (gitOptions.Repository == null)
		{
			logger.NoGitRepositoryConfigured();
			return GitCloneServiceStatus.NoRepository;
		}

		try
		{
			var remotes = await gitToolsPowershell.RunCommand(new GitRemote());

			if (remotes.Remotes.Count == 0)
			{
				logger.GitWithNoRemotes();
				return GitCloneServiceStatus.NoRemotes;
			}
			if (remotes.Remotes.Count > 1)
			{
				logger.MultipleGitRepositoriesConfigured(remotes.Remotes.Select(r => r.Alias));
				return GitCloneServiceStatus.MultipleRemotes;
			}

			if (remotes.Remotes[0].FetchUrl != gitOptions.Repository)
			{
				logger.GitRepositoryMismatch(expected: gitOptions.Repository, actual: remotes.Remotes[0].FetchUrl);
				return GitCloneServiceStatus.RepositoryMismatch;
			}

			logger.GitAlreadyCloned(remote: gitOptions.Repository, directory: gitOptions.WorkingDirectory);
			return GitCloneServiceStatus.AlreadyCloned;
		}
		catch (GitException)
		{
			// Exception encountered when listing remotes; hopefully is not a git directory
		}
		catch (Exception ex)
		{
			logger.GitFailedToCloneWithUnknownException(ex);
			throw;
		}

		try
		{
			await gitToolsPowershell.RunCommand(new GitClone(gitOptions.Repository));
			logger.GitClonedSuccessfully(gitOptions.Repository, gitOptions.WorkingDirectory);
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