using PrincipleStudios.ScaledGitApp.Git;
using PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

namespace PrincipleStudios.ScaledGitApp.BranchingStrategy;

public class ConflictLocator(IGitToolsCommandInvoker gitToolsPowerShell, IGitConfigurationService gitConfiguration) : IConflictLocator
{
	class ConflictLocatorContext
	{
		public Dictionary<BranchPair, IReadOnlyList<IdentifiedConflict>> Results { get; } = [];
		public required IReadOnlyDictionary<string, UpstreamBranchConfiguration> AllUpstreams { get; init; }

		internal IReadOnlyList<string> GetImmediateUpstream(string branch)
		{
			return AllUpstreams.TryGetValue(branch, out var config) ? config.UpstreamBranchNames : Array.Empty<string>();
		}
	}

	public async Task<IReadOnlyList<IdentifiedConflict>> FindConflictsWithin(IReadOnlyList<string> branches)
	{
		var results = new List<IdentifiedConflict>();
		var context = await ConstructContext();

		await GatherInitialResolutions(branches, context);

		for (var i = 0; i < branches.Count - 1; i++)
		{
			results.AddRange(await FindConflictsBetween([branches[i]], branches.Skip(i + 1).ToArray(), context));
		}
		return results.ToArray();
	}

	private static async Task GatherInitialResolutions(IReadOnlyList<string> branches, ConflictLocatorContext context)
	{
		for (var i = 0; i < branches.Count; i++)
		{
			var upstream = context.GetImmediateUpstream(branches[i]);
			if (upstream.Count == 2)
			{
				var pair = new BranchPair(upstream[0], upstream[1]);

				// TODO - get hash of branches[i]
				await Task.Yield();

				context.Results.Add(pair, [
					new IdentifiedConflict(pair, GetConflictingFilesResult.Empty("TODO", [branches[i]]))
				]);
			}
		}
	}

	public async Task<IReadOnlyList<IdentifiedConflict>> FindConflictsBetween(IReadOnlyList<string> leftBranches, IReadOnlyList<string> rightBranches)
	{
		var context = await ConstructContext();
		return await FindConflictsBetween(leftBranches, rightBranches, context);
	}

	private async Task<IReadOnlyList<IdentifiedConflict>> FindConflictsBetween(IReadOnlyList<string> leftBranches, IReadOnlyList<string> rightBranches, ConflictLocatorContext context)
	{
		var leftCommonUpstream = context.AllUpstreams.GetCommonUpstream(leftBranches);
		var rightCommonUpstream = context.AllUpstreams.GetCommonUpstream(rightBranches);

		var result = new List<IdentifiedConflict>();
		foreach (var left in leftBranches.Except(rightCommonUpstream))
			foreach (var right in rightBranches.Except(leftCommonUpstream))
			{
				var additionalConflicts = await FindConflictsBetween(new BranchPair(left, right), context);
				result.AddRange(additionalConflicts.Except(result).ToArray());
			}
		return result;
	}

	private async Task<IEnumerable<IdentifiedConflict>> FindConflictsBetween(BranchPair branchPair, ConflictLocatorContext context)
	{
		if (context.Results.TryGetValue(branchPair, out var cachedResult)) return cachedResult;

		var result = new List<IdentifiedConflict>();
		var returnValue = context.Results[branchPair] = result.AsReadOnly();

		var leftFullBranchName = await gitConfiguration.ToLocalTrackingBranchName(branchPair.LeftBranch);
		var rightFullBranchName = await gitConfiguration.ToLocalTrackingBranchName(branchPair.RightBranch);
		if (leftFullBranchName == null || rightFullBranchName == null) return returnValue;

		var conflictResult = await gitToolsPowerShell.RunCommand(new GetConflictingFiles(leftFullBranchName, rightFullBranchName, []));
		if (!conflictResult.HasConflict) return returnValue;

		var leftUpstream = context.GetImmediateUpstream(branchPair.LeftBranch);
		var rightUpstream = context.GetImmediateUpstream(branchPair.RightBranch);

		var subConflicts = await FindConflictsBetween(leftUpstream, rightUpstream, context);
		var actualSubConflicts = subConflicts.Where(c => c.ConflictingFiles.HasConflict).ToArray();

		if (actualSubConflicts.Length > 0)
			result.AddRange(actualSubConflicts);
		else
		{
			// retry the conflicts including the helper integration branches
			var includedIntegrationBranches = subConflicts.SelectMany(c => c.ConflictingFiles.IncludedIntegrationBranches).ToArray();
			conflictResult = await gitToolsPowerShell.RunCommand(new GetConflictingFiles(leftFullBranchName, rightFullBranchName, includedIntegrationBranches));
			result.Add(new(branchPair, conflictResult));
		}

		return returnValue;
	}

	private async Task<ConflictLocatorContext> ConstructContext()
	{
		var upstreams = await gitToolsPowerShell.RunCommand(new GitUpstreamData());

		return new ConflictLocatorContext
		{
			AllUpstreams = upstreams,
		};
	}
}

public record IdentifiedConflict(BranchPair Branches, GetConflictingFilesResult ConflictingFiles);