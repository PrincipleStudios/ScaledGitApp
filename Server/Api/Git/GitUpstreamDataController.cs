
using PrincipleStudios.ScaledGitApp.Git;
using PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

namespace PrincipleStudios.ScaledGitApp.Api.Git;

public class GitUpstreamDataController(IGitToolsInvoker GitToolsPowerShell) : GitUpstreamDataControllerBase
{

	protected override async Task<GetUpstreamDataActionResult> GetUpstreamData()
	{
		var results = await GitToolsPowerShell.RunCommand(new GitUpstreamData());
		var noUpstreams = results.SelectMany(r => r.Value.UpstreamBranchNames).Except(results.Keys);
		return GetUpstreamDataActionResult.Ok((
			from kvp in results
			select new BranchConfiguration(
				Name: kvp.Key,
				Color: DetermineColor(kvp.Key),
				Upstream: kvp.Value.UpstreamBranchNames.Select(n => new Branch(Name: n))
			)
		).Concat(
			from branchName in noUpstreams
			select new BranchConfiguration(
				Name: branchName,
				Color: DetermineColor(branchName),
				Upstream: Enumerable.Empty<Branch>()
			)
		));
	}

	private static string DetermineColor(string branchName)
	{
		// TODO: move to somewhere with configuration
		if (branchName.StartsWith("rc/")) return "rgb(111, 37, 111)";
		if (branchName.StartsWith("line/") || branchName == "main" || branchName == "master") return "rgb(111, 206, 31)";
		if (branchName.StartsWith("integ/") || branchName.StartsWith("integrate/") || branchName.StartsWith("integration/")) return "rgb(98, 98, 98)";
		return "rgb(55, 127, 192)";
	}
}
