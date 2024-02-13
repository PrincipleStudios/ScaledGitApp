using Microsoft.Extensions.Options;

namespace PrincipleStudios.ScaledGitApp.Git;

public class GitCloneService : IHostedService
{
	private readonly GitOptions gitOptions;
	private readonly IGitToolsPowershell gitToolsPowershell;
	private readonly ILogger<GitCloneService> logger;
	private Task? startupTask;

	public GitCloneService(IOptions<GitOptions> options, IGitToolsPowershell gitToolsPowershell, ILogger<GitCloneService> logger)
	{
		gitOptions = options.Value;
		this.gitToolsPowershell = gitToolsPowershell;
		this.logger = logger;
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		// Spin out to background worker so as to not block the main app startup
		startupTask = Task.Run(EnsureGitClone, cancellationToken);
		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	private async Task EnsureGitClone()
	{
		if (gitOptions.Repository == null)
		{
			logger.NoGitRepositoryConfigured();
			return;
		}

		try
		{
			var remotes = await gitToolsPowershell.GitRemote();

			if (remotes.Remotes.Count != 1)
			{
				logger.MultipleGitRepositoriesConfigured(remotes.Remotes.Select(r => r.Alias));
				return;
			}

			if (remotes.Remotes[0].FetchUrl != gitOptions.Repository)
			{
				logger.GitRepositoryMismatch(expected: gitOptions.Repository, actual: remotes.Remotes[0].FetchUrl);
				return;
			}

			logger.GitAlreadyCloned(remote: gitOptions.Repository, directory: gitOptions.WorkingDirectory);
			return;
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
			await gitToolsPowershell.GitClone(gitOptions.Repository);
			logger.GitClonedSuccessfully(gitOptions.Repository, gitOptions.WorkingDirectory);
		}
		catch (GitException ex)
		{
			logger.GitFailedToClone(ex);
		}
		catch (Exception ex)
		{
			logger.GitFailedToCloneWithUnknownException(ex);
			throw;
		}
	}
}
