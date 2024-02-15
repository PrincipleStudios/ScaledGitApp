
using PrincipleStudios.ScaledGitApp.Git;

namespace PrincipleStudios.ScaledGitApp.Api.Git;

public class GitFetchController : GitFetchControllerBase
{
	private readonly IGitToolsPowerShell gitToolsPowerShell;

	public GitFetchController(IGitToolsPowerShell gitToolsPowerShell)
	{
		this.gitToolsPowerShell = gitToolsPowerShell;
	}

	protected override async Task<RequestGitFetchActionResult> RequestGitFetch()
	{
		await gitToolsPowerShell.GitFetch();
		return RequestGitFetchActionResult.Ok();
	}
}
