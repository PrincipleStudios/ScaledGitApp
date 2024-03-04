using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

public record GitBranchUpstreamDetails(string BranchName) : IGitToolsCommand<Task<UpstreamBranchDetailedState>>
{
	public async Task<UpstreamBranchDetailedState> RunCommand(IGitToolsPowerShellCommandContext pwsh)
	{
		var upstreamResults = await pwsh.InvokeCliAsync("git", ["cat-file", "-p", $"{pwsh.UpstreamBranchName}:{BranchName}"]);
		var upstreamBranches = upstreamResults.HadErrors
			? Array.Empty<string>()
			: upstreamResults.ToResultStrings().ToArray();

		var fullBranchName = pwsh.ToLocalTrackingBranchName(BranchName)
			?? throw new InvalidOperationException($"{BranchName} is not mapped locally");
		var branchExistenceCheck = await pwsh.InvokeCliAsync("git", ["rev-parse", "--verify", fullBranchName]);
		var branchExists = !branchExistenceCheck.HadErrors;

		var fullUpstreamBranchNames = upstreamBranches
			.Select(abbreviated => (ShortName: abbreviated, FullName: pwsh.ToLocalTrackingBranchName(abbreviated)
				?? throw new InvalidOperationException($"{abbreviated} is not mapped locally")))
			.ToArray();

		var entries = new List<UpstreamBranchMergeInfo>();
		foreach (var upstream in fullUpstreamBranchNames)
		{
			var behind = await pwsh.RunCommand(new GetCommitCount(Included: [upstream.FullName], Excluded: (IEnumerable<string>)([fullBranchName])));
			entries.Add(new UpstreamBranchMergeInfo(
				Name: upstream.ShortName,
				Exists: !behind.HasValue,
				BehindCount: behind ?? 0,
				HasConflict: await pwsh.RunCommand(new GetConflictingFiles(upstream.FullName, fullBranchName))
			));
		}

		return new UpstreamBranchDetailedState(
			Exists: branchExists,
			NonMergeCommitCount: branchExists
				? await pwsh.RunCommand(new GetCommitCount(Included: [fullBranchName], Excluded: fullUpstreamBranchNames.Select(n => n.FullName))) ?? 0
				: 0,
			Upstreams: entries.ToArray()
		);
	}
}

public record UpstreamBranchDetailedState(bool Exists, int NonMergeCommitCount, IEnumerable<UpstreamBranchMergeInfo> Upstreams);
public record UpstreamBranchMergeInfo(string Name, bool Exists, int BehindCount, bool HasConflict);
