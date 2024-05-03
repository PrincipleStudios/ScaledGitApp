using PrincipleStudios.ScaledGitApp.Api.Git.Conversions;
using PrincipleStudios.ScaledGitApp.BranchingStrategy;
using PrincipleStudios.ScaledGitApp.Git;
using PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

namespace PrincipleStudios.ScaledGitApp.Api.Git;

public class GitDetectConflictsController(IGitToolsCommandInvoker gitToolsPowerShell, IBranchTypeLookup branchTypeLookup, IGitConfigurationService gitConfiguration, IBranchDetailsMapper branchDetailsMapper) : GitDetectConflictsControllerBase
{
	protected override async Task<GetConflictDetailsActionResult> GetConflictDetails(GetConflictDetailsRequest getConflictDetailsBody)
	{
		var branches = getConflictDetailsBody.Branches.ToArray();
		if (branches.Length != 2)
			return GetConflictDetailsActionResult.BadRequest();

		var upstreams = await gitToolsPowerShell.RunCommand(new GitUpstreamData());

		// Determine the upstream relevant branches - if there are fully-shared
		// upstreams, they aren't relevant. (An infra branch shared by all
		// branches specified won't cause new conflicts.)
		var allUpstreamLookup = branches.ToDictionary(b => b, b => GetAllUpstream(b, upstreams));
		var commonUpstreams = allUpstreamLookup.Values.Aggregate((IEnumerable<string> prev, IEnumerable<string> next) => prev.Intersect(next)).ToArray();
		var relevantUpstreamLookup = allUpstreamLookup.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Except(commonUpstreams).ToArray());

		var relevantBranches = relevantUpstreamLookup.Values.SelectMany(b => b).Distinct();

		// Check to see if updates need to be pulled for any of the relevant
		// branches before checking for conflicts. Not having everything
		// up-to-date can hide where the conflicts actually belong.
		var allBranchUpstreamDetails = await GetBranchDetails(relevantBranches);
		var outOfDate = allBranchUpstreamDetails.Where(b => b.Upstreams.Any(u => u.HasConflict || u.BehindCount > 0)).ToArray();
		if (outOfDate.Length > 0) return GetConflictDetailsActionResult.Conflict(outOfDate.Select(branchDetailsMapper.ToBranchDetails));

		// Determine if the named branches actually have conflicts
		var conflictDetails = await GetConflictDetails(branches[0], branches[1]);
		if (conflictDetails == null) return GetConflictDetailsActionResult.Ok(Enumerable.Empty<ConflictDetails>());

		// TODO: Naive implementation; needs to check upstreams
		return GetConflictDetailsActionResult.Ok([conflictDetails]);
	}

	private string[] GetAllUpstream(string branch, IReadOnlyDictionary<string, UpstreamBranchConfiguration> upstreams)
	{
		var allBranches = new HashSet<string>([branch]);
		var stack = new Stack<string>([branch]);
		while (stack.TryPop(out var current))
		{
			if (!upstreams.TryGetValue(current, out var upstreamConfig)) continue;
			foreach (var upstream in upstreamConfig.UpstreamBranchNames)
			{
				if (allBranches.Contains(upstream)) continue;

				allBranches.Add(upstream);
				stack.Push(upstream);
			}
		}
		return allBranches.ToArray();
	}

	private async Task<UpstreamBranchDetailedState[]> GetBranchDetails(IEnumerable<string> branches)
	{
		var result = new List<UpstreamBranchDetailedState>();
		foreach (var current in branches)
		{
			var next = await gitToolsPowerShell.RunCommand(new GitBranchUpstreamDetails(current, IncludeDownstream: false, IncludeUpstream: false, Recurse: false));
			var state = next[0];
			result.Add(state);
		}

		return result.ToArray();
	}

	private async Task<ConflictDetails?> GetConflictDetails(string leftBranch, string rightBranch)
	{
		var leftFullBranchName = await gitConfiguration.ToLocalTrackingBranchName(leftBranch);
		var rightFullBranchName = await gitConfiguration.ToLocalTrackingBranchName(rightBranch);
		if (leftFullBranchName == null || rightFullBranchName == null) return null;

		var result = await gitToolsPowerShell.RunCommand(new GetConflictingFiles(leftFullBranchName, rightFullBranchName));
		if (!result.HasConflict) return null;


		return new ConflictDetails(new[] { leftBranch, rightBranch }.Select(ToBranch), result.ConflictingFileNames.Select(ToFileDetails));
	}

	private Branch ToBranch(string branchName)
	{
		var info = branchTypeLookup.DetermineBranchType(branchName);
		return new Branch(Name: branchName, Color: info.Color, Type: info.BranchType);
	}

	private FileConflictDetails ToFileDetails(string path)
	{
		return new FileConflictDetails(Path: path);
	}
}
