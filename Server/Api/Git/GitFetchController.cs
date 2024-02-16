
using PrincipleStudios.ScaledGitApp.Git;
using PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

namespace PrincipleStudios.ScaledGitApp.Api.Git;

public class GitFetchController(IGitToolsInvoker GitToolsPowerShell) : GitFetchControllerBase
{
	protected override async Task<RequestGitFetchActionResult> RequestGitFetch()
	{
		await GitToolsPowerShell.RunCommand(new GitFetch());
		return RequestGitFetchActionResult.Ok();
	}
}
