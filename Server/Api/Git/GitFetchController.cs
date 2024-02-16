
using PrincipleStudios.ScaledGitApp.Git;
using PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

namespace PrincipleStudios.ScaledGitApp.Api.Git;

public class GitFetchController : GitFetchControllerBase
{
	private readonly IGitToolsInvoker gitToolsPowerShell;

	public GitFetchController(IGitToolsInvoker gitToolsPowerShell)
	{
		this.gitToolsPowerShell = gitToolsPowerShell;
	}

	protected override async Task<RequestGitFetchActionResult> RequestGitFetch()
	{
		await gitToolsPowerShell.RunCommand(new GitFetch());
		return RequestGitFetchActionResult.Ok();
	}
}
