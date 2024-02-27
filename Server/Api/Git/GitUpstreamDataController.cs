
using PrincipleStudios.ScaledGitApp.Git;
using PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

namespace PrincipleStudios.ScaledGitApp.Api.Git;

public class GitUpstreamDataController(IGitToolsInvoker GitToolsPowerShell) : GitUpstreamDataControllerBase
{

	protected override async Task<GetUpstreamDataActionResult> GetUpstreamData()
	{
		var results = await GitToolsPowerShell.RunCommand(new GitUpstreamData());
		return GetUpstreamDataActionResult.Ok(results.ToDictionary(e => e.Key, e => e.Value.UpstreamBranchNames.Select(b => new Branch(Name: b))));
	}
}
