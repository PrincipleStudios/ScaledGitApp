using PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

namespace PrincipleStudios.ScaledGitApp.Api.Git;

public class GitFetchController(IGitToolsCommandInvoker GitToolsPowerShell) : GitFetchControllerBase
{
	protected override async Task<RequestGitFetchActionResult> RequestGitFetch()
	{
		await GitToolsPowerShell.RunCommand(new GitFetch());
		return RequestGitFetchActionResult.Ok();
	}
}
