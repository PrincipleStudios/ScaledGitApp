﻿namespace PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

public record GitBranchUpstreamDetails(IReadOnlyList<string> BranchNames, bool IncludeDownstream, bool IncludeUpstream, bool Recurse, int? Limit = null)
	: IGitToolsCommand<Task<IReadOnlyList<UpstreamBranchDetailedState>>>
{
	public async Task<IReadOnlyList<UpstreamBranchDetailedState>> RunCommand(IGitToolsCommandContext pwsh)
	{
		var upstreams = await pwsh.RunCommand(new GitUpstreamData());
		var baseBranches = ExpandBaseBranches(upstreams, BranchNames);

		var fullNames = new Dictionary<string, string>();
		var existence = new Dictionary<string, bool>();

		var result = new List<UpstreamBranchDetailedState>();
		foreach (var baseBranch in baseBranches)
		{
			var fullBranchName = FullName(baseBranch);
			var branchExists = await CheckBranchExists(fullBranchName);

			var upstreamBranches = upstreams.TryGetValue(baseBranch, out var upstream)
				? upstream.UpstreamBranchNames
				: Enumerable.Empty<string>();

			var entries = new List<UpstreamBranchMergeInfo>();
			foreach (var shortUpstream in upstreamBranches)
			{
				var fullUpstream = FullName(shortUpstream);

				if (!branchExists)
				{
					entries.Add(new UpstreamBranchMergeInfo(
						Name: shortUpstream,
						Exists: await CheckBranchExists(fullUpstream),
						BehindCount: 0,
						HasConflict: false
					));
					continue;
				}

				var behind = await pwsh.RunCommand(new GetCommitCount(Included: [fullUpstream], Excluded: [fullBranchName]));
				entries.Add(new UpstreamBranchMergeInfo(
					Name: shortUpstream,
					Exists: behind.HasValue,
					BehindCount: behind ?? 0,
					HasConflict: behind.HasValue && (await pwsh.RunCommand(new GetConflictingFiles(fullUpstream, fullBranchName))).HasConflict
				));
			}

			result.Add(new(
				Name: baseBranch,
				Exists: branchExists,
				NonMergeCommitCount: branchExists
					? await pwsh.RunCommand(new GetCommitCount(
						Included: [fullBranchName],
						Excluded: from entry in entries
								  where entry.Exists
								  select FullName(entry.Name)
					)) ?? 0
					: 0,
				Upstreams: entries.ToArray(),
				DownstreamNames: GetDownstreamBranchNames(upstreams, baseBranch).ToArray()
			));
		}

		return result.AsReadOnly();

		string FullName(string branchName)
		{
			if (fullNames.TryGetValue(branchName, out var name)) return name;
			var result = pwsh.GitCloneConfiguration.ToLocalTrackingBranchName(branchName)
				?? throw new InvalidOperationException($"{branchName} is not mapped locally");
			fullNames.Add(branchName, result);
			return result;
		}
		async Task<bool> CheckBranchExists(string branchName)
		{
			if (existence.TryGetValue(branchName, out var result)) return result;

			var branchExists = await pwsh.RunCommand(new BranchExists(branchName));
			existence.Add(branchName, branchExists);
			return branchExists;
		}
	}

	private string[] ExpandBaseBranches(IReadOnlyDictionary<string, UpstreamBranchConfiguration> upstreams, IReadOnlyList<string> branchNames)
	{
		IEnumerable<string> result = branchNames;
		if (IncludeDownstream)
			result = result.Concat(ExpandBaseBranches(branchNames, (current) =>
					GetDownstreamBranchNames(upstreams, current)));
		if (IncludeUpstream)
			result = result.Concat(ExpandBaseBranches(branchNames, (current) =>
					upstreams.TryGetValue(current, out var configuredUpstreams)
						? configuredUpstreams.UpstreamBranchNames
						: Enumerable.Empty<string>()));
		result = result.Distinct();
		if (Limit is int limit)
			result = result.Take(limit);
		return result.ToArray();
	}

	private static IEnumerable<string> GetDownstreamBranchNames(IReadOnlyDictionary<string, UpstreamBranchConfiguration> upstreams, string current)
	{
		return from kvp in upstreams
			   where kvp.Value.UpstreamBranchNames.Contains(current)
			   select kvp.Key;
	}

	private string[] ExpandBaseBranches(IReadOnlyList<string> branchNames, Func<string, IEnumerable<string>> getMore)
	{
		var result = new HashSet<string>(branchNames);
		var stack = new Stack<string>(branchNames);
		while (stack.TryPop(out var current))
		{
			var adding = getMore(current);

			foreach (var entry in adding)
			{
				if (result.Contains(entry)) continue;
				result.Add(entry);
				if (Recurse)
					stack.Push(entry);
			}
		}
		return result.ToArray();
	}
}

public record UpstreamBranchDetailedState(string Name, bool Exists, int NonMergeCommitCount, IEnumerable<UpstreamBranchMergeInfo> Upstreams, IReadOnlyList<string> DownstreamNames);
public record UpstreamBranchMergeInfo(string Name, bool Exists, int BehindCount, bool HasConflict);
