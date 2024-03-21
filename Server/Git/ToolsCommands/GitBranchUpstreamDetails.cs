namespace PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

public record GitBranchUpstreamDetails(IReadOnlyList<string> BranchNames, bool IncludeDownstream, bool IncludeUpstream, bool Recurse, int? Limit = null)
	: IGitToolsCommand<Task<IReadOnlyList<UpstreamBranchDetailedState>>>
{
	public async Task<IReadOnlyList<UpstreamBranchDetailedState>> RunCommand(IGitToolsCommandContext pwsh)
	{
		var upstreams = await pwsh.RunCommand(new GitUpstreamData());
		var context = new ExecutionContext(pwsh, upstreams);
		var baseBranches = ExpandBaseBranches(context, BranchNames);

		var result = new List<UpstreamBranchDetailedState>();
		foreach (var baseBranch in baseBranches)
		{
			var fullBranchName = context.FullName(baseBranch);
			var branchExists = await context.CheckBranchExists(fullBranchName);

			var entries = await LoadImmediateUpstreamInfo(context, baseBranch, branchExists);

			result.Add(new(
				Name: baseBranch,
				Exists: branchExists,
				NonMergeCommitCount: branchExists
					? await pwsh.RunCommand(new GetCommitCount(
						Included: [fullBranchName],
						Excluded: from entry in entries
								  where entry.Exists
								  select context.FullName(entry.Name)
					)) ?? 0
					: 0,
				Upstreams: entries.ToArray(),
				DownstreamNames: GetDownstreamBranchNames(context, baseBranch).ToArray()
			));
		}

		return result.AsReadOnly();
	}

	private static async Task<List<UpstreamBranchMergeInfo>> LoadImmediateUpstreamInfo(ExecutionContext context, string baseBranch, bool branchExists)
	{
		var fullBranchName = context.FullName(baseBranch);
		var upstreamBranches = context.Upstreams.TryGetValue(baseBranch, out var upstream)
			? upstream.UpstreamBranchNames
			: Enumerable.Empty<string>();

		var entries = new List<UpstreamBranchMergeInfo>();
		foreach (var shortUpstream in upstreamBranches)
		{
			var fullUpstream = context.FullName(shortUpstream);

			if (!branchExists)
			{
				entries.Add(new UpstreamBranchMergeInfo(
					Name: shortUpstream,
					Exists: await context.CheckBranchExists(fullUpstream),
					BehindCount: 0,
					HasConflict: false
				));
				continue;
			}

			var behind = await context.Pwsh.RunCommand(new GetCommitCount(Included: [fullUpstream], Excluded: [fullBranchName]));
			entries.Add(new UpstreamBranchMergeInfo(
				Name: shortUpstream,
				Exists: behind.HasValue,
				BehindCount: behind ?? 0,
				HasConflict: behind.HasValue && (await context.Pwsh.RunCommand(new GetConflictingFiles(fullUpstream, fullBranchName))).HasConflict
			));
		}

		return entries;
	}

	private class ExecutionContext(
		IGitToolsCommandContext pwsh,
		IReadOnlyDictionary<string, UpstreamBranchConfiguration> upstreams)
	{
		public IGitToolsCommandContext Pwsh => pwsh;
		public Dictionary<string, string> FullNames { get; } = new();
		public Dictionary<string, bool> Existence { get; } = new();
		public IReadOnlyDictionary<string, UpstreamBranchConfiguration> Upstreams => upstreams;

		public string FullName(string branchName)
		{
			if (FullNames.TryGetValue(branchName, out var name)) return name;
			var result = pwsh.GitCloneConfiguration.ToLocalTrackingBranchName(branchName)
				?? throw new InvalidOperationException($"{branchName} is not mapped locally");
			FullNames.Add(branchName, result);
			return result;
		}

		public async Task<bool> CheckBranchExists(string branchName)
		{
			if (Existence.TryGetValue(branchName, out var result)) return result;

			var branchExists = await pwsh.RunCommand(new BranchExists(branchName));
			Existence.Add(branchName, branchExists);
			return branchExists;
		}
	}

	private string[] ExpandBaseBranches(ExecutionContext context, IReadOnlyList<string> branchNames)
	{
		IEnumerable<string> result = branchNames;
		if (IncludeDownstream)
			result = result.Concat(ExpandBaseBranches(branchNames, (current) =>
					GetDownstreamBranchNames(context, current)));
		if (IncludeUpstream)
			result = result.Concat(ExpandBaseBranches(branchNames, (current) =>
					context.Upstreams.TryGetValue(current, out var configuredUpstreams)
						? configuredUpstreams.UpstreamBranchNames
						: Enumerable.Empty<string>()));
		result = result.Distinct();
		if (Limit is int maxLimit)
			result = result.Take(maxLimit);
		return result.ToArray();
	}

	private static IEnumerable<string> GetDownstreamBranchNames(ExecutionContext context, string current)
	{
		return from kvp in context.Upstreams
			   where kvp.Value.UpstreamBranchNames.Contains(current)
			   select kvp.Key;
	}

	private string[] ExpandBaseBranches(IReadOnlyList<string> branchNames, Func<string, IEnumerable<string>> getMore, bool forceRecurse = false)
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
				if (Recurse || forceRecurse)
					stack.Push(entry);
			}
		}
		return result.ToArray();
	}
}

public record UpstreamBranchDetailedState(string Name, bool Exists, int NonMergeCommitCount, IEnumerable<UpstreamBranchMergeInfo> Upstreams, IReadOnlyList<string> DownstreamNames);
public record UpstreamBranchMergeInfo(string Name, bool Exists, int BehindCount, bool HasConflict);
