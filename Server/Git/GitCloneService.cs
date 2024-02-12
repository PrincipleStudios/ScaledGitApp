using Microsoft.Extensions.Options;
using System.Collections;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace PrincipleStudios.ScaledGitApp.Git;

public class GitCloneService : IHostedService
{
	private readonly GitOptions gitOptions;
	private readonly GitToolsPowershell gitToolsPowershell;

	public GitCloneService(IOptions<GitOptions> options, GitToolsPowershell gitToolsPowershell)
	{
		gitOptions = options.Value;
		this.gitToolsPowershell = gitToolsPowershell;
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

			Console.WriteLine(powershellResponse.Results);
		}
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}
}
