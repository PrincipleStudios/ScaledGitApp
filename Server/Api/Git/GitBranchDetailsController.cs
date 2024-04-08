using PrincipleStudios.OpenApiCodegen.Json.Extensions;
using PrincipleStudios.ScaledGitApp.BranchingStrategy;
using PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

namespace PrincipleStudios.ScaledGitApp.Api.Git;

public class GitBranchDetailsController(IGitToolsCommandInvoker gitToolsPowerShell, IBranchTypeLookup branchTypeLookup) : GitBranchDetailsControllerBase
{
	protected override async Task<GetBranchDetailsActionResult> GetBranchDetails(GetBranchDetailsRequest getBranchDetailsBody)
	{
		var results = await gitToolsPowerShell.RunCommand(
			new GitBranchUpstreamDetails(
				getBranchDetailsBody.Branch,
				IncludeDownstream: false,
				IncludeUpstream: false,
				Recurse: false,
				Limit: null
			)
		);

		return GetBranchDetailsActionResult.Ok(ToBranchDetails(results.Single()));
	}

	private BranchDetails ToBranchDetails(UpstreamBranchDetailedState result)
	{
		var type = branchTypeLookup.DetermineBranchType(result.Name);

		return new BranchDetails(
			result.Name,
			Type: type.BranchType,
			Color: type.Color,
			Exists: result.Exists,
			NonMergeCommitCount: result.NonMergeCommitCount,
			Upstream: from upstream in result.Upstreams
					  let upstreamType = branchTypeLookup.DetermineBranchType(upstream.Name)
					  select new DetailedUpstreamBranch(
						  Name: upstream.Name,
						  Type: upstreamType.BranchType,
						  Color: upstreamType.Color,
						  Exists: upstream.Exists,
						  BehindCount: upstream.BehindCount,
						  HasConflict: upstream.HasConflict
					  ),
			Downstream: from downstream in result.DownstreamNames
						select new Branch(Name: downstream)
		);
	}
}
