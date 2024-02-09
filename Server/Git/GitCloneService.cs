
using Microsoft.Extensions.Options;

namespace PrincipleStudios.ScaledGitApp.Git;

public class GitCloneService : IHostedService
{
	private readonly GitOptions gitOptions;

	public GitCloneService(IOptions<GitOptions> options)
	{
		gitOptions = options.Value;
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		if (gitOptions.Repository != null)
		{
			// TODO: clone {gitOptions.Repository} in {gitOptions.WorkingDirectory}
		}
		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}
}
