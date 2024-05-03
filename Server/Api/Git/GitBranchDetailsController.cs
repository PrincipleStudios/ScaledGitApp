using PrincipleStudios.ScaledGitApp.Api.Git.Conversions;
using PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

namespace PrincipleStudios.ScaledGitApp.Api.Git;

public class GitBranchDetailsController(IGitToolsCommandInvoker gitToolsPowerShell, IBranchDetailsMapper branchDetailsMapper) : GitBranchDetailsControllerBase
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

		return GetBranchDetailsActionResult.Ok(branchDetailsMapper.ToBranchDetails(results.Single()));
	}
}
