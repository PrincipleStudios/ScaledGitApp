
using PrincipleStudios.ScaledGitApp.BranchingStrategy;
using PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

namespace PrincipleStudios.ScaledGitApp.Api.Git;

public class GitUpstreamDataController(IGitToolsCommandInvoker gitToolsPowerShell, IBranchTypeLookup branchTypeLookup) : GitUpstreamDataControllerBase
{

	protected override async Task<GetUpstreamDataActionResult> GetUpstreamData()
	{
		var results = await gitToolsPowerShell.RunCommand(new GitUpstreamData());
		var noUpstreams = results.SelectMany(r => r.Value.UpstreamBranchNames).Except(results.Keys);
		return GetUpstreamDataActionResult.Ok((
			from kvp in results
			let type = branchTypeLookup.DetermineBranchType(kvp.Key)
			select new BranchConfiguration(
				Name: kvp.Key,
				Type: type.BranchType,
				Color: type.Color,
				Upstream: from upstream in kvp.Value.UpstreamBranchNames
						  let upstreamType = branchTypeLookup.DetermineBranchType(upstream)
						  select new Branch(
							  Name: upstream,
							  Type: upstreamType.BranchType,
							  Color: upstreamType.Color
						  ),
				Downstream: from entry in results
							where entry.Value.UpstreamBranchNames.Contains(kvp.Key)
							let downstreamType = branchTypeLookup.DetermineBranchType(entry.Key)
							select new Branch(Name: entry.Key, Color: downstreamType.Color, Type: downstreamType.BranchType)
			)
		).Concat(
			from branchName in noUpstreams
			let type = branchTypeLookup.DetermineBranchType(branchName)
			select new BranchConfiguration(
				Name: branchName,
				Type: type.BranchType,
				Color: type.Color,
				Upstream: Enumerable.Empty<Branch>(),
				Downstream: from entry in results
							where entry.Value.UpstreamBranchNames.Contains(branchName)
							let downstreamType = branchTypeLookup.DetermineBranchType(entry.Key)
							select new Branch(Name: entry.Key, Color: downstreamType.Color, Type: downstreamType.BranchType)
			)
		));
	}
}
