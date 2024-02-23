﻿
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
			select new UpstreamBranchConfiguration(
				Name: kvp.Key,
				Upstream: kvp.Value.UpstreamBranchNames.Select(n => new Branch(Name: n))
			)
		).Concat(
			from branchName in noUpstreams
			select new UpstreamBranchConfiguration(
				Name: branchName,
				Upstream: Enumerable.Empty<Branch>()
			)
		));
	}
}
