
using PrincipleStudios.ScaledGitApp.BranchingStrategy;
using PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

namespace PrincipleStudios.ScaledGitApp.Api.Git;

public class GitUpstreamDataController(IGitToolsCommandInvoker GitToolsPowerShell, IBranchTypeLookup ColorConfiguration) : GitUpstreamDataControllerBase
{

	protected override async Task<GetUpstreamDataActionResult> GetUpstreamData()
	{
		var results = await GitToolsPowerShell.RunCommand(new GitUpstreamData());
		var noUpstreams = results.SelectMany(r => r.Value.UpstreamBranchNames).Except(results.Keys);
		return GetUpstreamDataActionResult.Ok((
			from kvp in results
			let type = ColorConfiguration.DetermineBranchType(kvp.Key)
			select new BranchConfiguration(
				Name: kvp.Key,
				Type: type.BranchType,
				Color: type.Color,
				Upstream: kvp.Value.UpstreamBranchNames.Select(n => new Branch(Name: n)),
				Downstream: from entry in results
							where entry.Value.UpstreamBranchNames.Contains(kvp.Key)
							select new Branch(Name: entry.Key)
			)
		).Concat(
			from branchName in noUpstreams
			let type = ColorConfiguration.DetermineBranchType(branchName)
			select new BranchConfiguration(
				Name: branchName,
				Type: type.BranchType,
				Color: type.Color,
				Upstream: Enumerable.Empty<Branch>(),
				Downstream: from entry in results
							where entry.Value.UpstreamBranchNames.Contains(branchName)
							select new Branch(Name: entry.Key)
			)
		));
	}
}
