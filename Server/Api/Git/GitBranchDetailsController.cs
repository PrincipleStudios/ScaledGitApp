
using PrincipleStudios.ScaledGitApp.BranchingStrategy;
using PrincipleStudios.ScaledGitApp.Git;
using PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

namespace PrincipleStudios.ScaledGitApp.Api.Git;

public class GitBranchDetailsController(IGitToolsCommandInvoker GitToolsPowerShell, IColorConfiguration ColorConfiguration) : GitBranchDetailsControllerBase
{
	protected override async Task<GetBranchDetailsActionResult> GetBranchDetails(string branch)
	{
		var result = await GitToolsPowerShell.RunCommand(new GitBranchUpstreamDetails(branch));


		return GetBranchDetailsActionResult.Ok(new BranchDetails(
			branch,
			ColorConfiguration.DetermineColor(branch),
			Exists: result.Exists,
			NonMergeCommitCount: result.NonMergeCommitCount,
			Upstream: from upstream in result.Upstreams
					  select new DetailedUpstreamBranch(
						  Name: upstream.Name,
						  Color: ColorConfiguration.DetermineColor(upstream.Name),
						  Exists: upstream.Exists,
						  BehindCount: upstream.BehindCount,
						  HasConflict: upstream.HasConflict
					  )
		));
	}
}
