
using PrincipleStudios.ScaledGitApp.BranchingStrategy;
using PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

namespace PrincipleStudios.ScaledGitApp.Api.Git;

public class GitUpstreamDataController(IGitToolsCommandInvoker GitToolsPowerShell, IColorConfiguration ColorConfiguration) : GitUpstreamDataControllerBase
{

	protected override async Task<GetUpstreamDataActionResult> GetUpstreamData()
	{
		var results = await GitToolsPowerShell.RunCommand(new GitUpstreamData());
		var noUpstreams = results.SelectMany(r => r.Value.UpstreamBranchNames).Except(results.Keys);
		return GetUpstreamDataActionResult.Ok((
			from kvp in results
			select new BranchConfiguration(
				Name: kvp.Key,
				Color: ColorConfiguration.DetermineColor(kvp.Key),
				Upstream: kvp.Value.UpstreamBranchNames.Select(n => new Branch(Name: n))
			)
		).Concat(
			from branchName in noUpstreams
			select new BranchConfiguration(
				Name: branchName,
				Color: ColorConfiguration.DetermineColor(branchName),
				Upstream: Enumerable.Empty<Branch>()
			)
		));
	}
}
