using PrincipleStudios.OpenApiCodegen.Json.Extensions;
using PrincipleStudios.ScaledGitApp.BranchingStrategy;
using PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

namespace PrincipleStudios.ScaledGitApp.Api.Git;

public class GitBranchDetailsController(IGitToolsCommandInvoker gitToolsPowerShell, IColorConfiguration colorConfiguration) : GitBranchDetailsControllerBase
{
	protected override async Task<GetBranchDetailsActionResult> GetBranchDetails(GetBranchDetailsRequest getBranchDetailsBody)
	{
		var results = await gitToolsPowerShell.RunCommand(
			new GitBranchUpstreamDetails(
				getBranchDetailsBody.Branches.ToArray(),
				IncludeDownstream: getBranchDetailsBody.IncludeDownstream,
				IncludeUpstream: getBranchDetailsBody.IncludeUpstream,
				Recurse: getBranchDetailsBody.Recurse,
				Limit: getBranchDetailsBody.Limit.TryGet(out var limit) ? limit : null
			)
		);

		return GetBranchDetailsActionResult.Ok(results.Select(ToBranchDetails));
	}

	private BranchDetails ToBranchDetails(UpstreamBranchDetailedState result) =>
		new BranchDetails(
			result.Name,
			colorConfiguration.DetermineColor(result.Name),
			Exists: result.Exists,
			NonMergeCommitCount: result.NonMergeCommitCount,
			Upstream: from upstream in result.Upstreams
					  select new DetailedUpstreamBranch(
						  Name: upstream.Name,
						  Color: colorConfiguration.DetermineColor(upstream.Name),
						  Exists: upstream.Exists,
						  BehindCount: upstream.BehindCount,
						  HasConflict: upstream.HasConflict
					  ),
			Downstream: result.DownstreamNames
		);
}
