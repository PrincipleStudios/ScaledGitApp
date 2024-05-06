using PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

namespace PrincipleStudios.ScaledGitApp.BranchingStrategy;

/// <summary>
/// A suite of extension methods to work with upstream data. These cases are
/// well-reasoned and unit tested, so do not need to be mocked.
/// </summary>
public static class UpstreamUtilities
{
	public static string[] GetAllUpstream(this IReadOnlyDictionary<string, UpstreamBranchConfiguration> upstreams, string branch)
	{
		var allBranches = new HashSet<string>();
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
}
