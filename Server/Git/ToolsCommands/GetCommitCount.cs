using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

public record GetCommitCount(IEnumerable<string> Included, IEnumerable<string> Excluded, bool ExcludeMergeCommits = true) : IPowerShellCommand<Task<int?>>
{
	private static readonly IReadOnlyList<string> countWithoutMergesArgs = ["rev-list", "--count", "--no-merges"];
	private static readonly IReadOnlyList<string> countWithMergesArgs = ["rev-list", "--count"];

	public async Task<int?> Execute(IPowerShellCommandContext context)
	{
		var countArgs = ExcludeMergeCommits ? countWithoutMergesArgs : countWithMergesArgs;
		var cliResults = await context.InvokeCliAsync("git", countArgs.Concat(Included).Concat(Excluded.Select(b => $"^{b}")).ToArray());
		if (cliResults.HadErrors) return null;
		return int.Parse(cliResults.ToResultStrings().Single());
	}

	public virtual bool Equals(GetCommitCount? other) =>
		other?.GetType() == GetType() && Included.SequenceEqual(other.Included) && Excluded.SequenceEqual(other.Excluded);
	public override int GetHashCode() =>
		Included.Concat(Excluded).Aggregate(typeof(GetCommitCount).GetHashCode(), (prev, next) => prev ^ next.GetHashCode());
}
