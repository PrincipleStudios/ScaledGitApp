using Microsoft.Extensions.Options;

namespace PrincipleStudios.ScaledGitApp.Git;

public class GitCloneService : IHostedService
{
	private readonly GitOptions gitOptions;
	private readonly GitToolsPowershell gitToolsPowershell;
	private readonly ILogger<GitCloneService> logger;

	public GitCloneService(IOptions<GitOptions> options, GitToolsPowershell gitToolsPowershell, ILogger<GitCloneService> logger)
	{
		gitOptions = options.Value;
		this.gitToolsPowershell = gitToolsPowershell;
		this.logger = logger;
	}

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		// TODO: verify configuration then spin out to background worker so as to not block the main app startup

		if (gitOptions.Repository != null)
		{
			// TODO: clone {gitOptions.Repository} in {gitOptions.WorkingDirectory}

			var powershellResponse = await gitToolsPowershell.InvokeGitToolsAsync("git-show-upstream.ps1", c => c
				.AddParameter("target", "git-setup")
				.AddParameter("recurse"));

#pragma warning disable CA1848 // Use the LoggerMessage delegates
			logger.LogInformation("Temp: pwsh response: {Response}", powershellResponse);
#pragma warning restore CA1848 // Use the LoggerMessage delegates
		}
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}
}
