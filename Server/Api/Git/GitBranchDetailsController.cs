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
				[getBranchDetailsBody.Branch],
				IncludeDownstream: false,
				IncludeUpstream: false,
				Recurse: false,
				Limit: null
			)
		);

		return GetBranchDetailsActionResult.Ok(ToBranchDetails(results.Single()));
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
			Downstream: from downstream in result.DownstreamNames
						select new Branch(Name: downstream)
		);
}
