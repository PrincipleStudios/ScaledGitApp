using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

public record GetCommitCount(IEnumerable<string> Included, IEnumerable<string> Excluded) : IPowerShellCommand<Task<int?>>
{
	private static readonly IReadOnlyList<string> countArgs = ["rev-list", "--count", "--no-merges"];

	public async Task<int?> Execute(IPowerShellCommandContext context)
	{
		var cliResults = await context.InvokeCliAsync("git", countArgs.Concat(Included).Concat(Excluded.Select(b => $"^{b}")).ToArray());
		if (cliResults.HadErrors) return null;
		return int.Parse(cliResults.ToResultStrings().Single());
	}

	public virtual bool Equals(GetCommitCount? other) =>
		other?.GetType() == GetType() && Included.SequenceEqual(other.Included) && Excluded.SequenceEqual(other.Excluded);
	public override int GetHashCode() =>
		Included.Concat(Excluded).Aggregate(typeof(GetCommitCount).GetHashCode(), (prev, next) => prev ^ next.GetHashCode());
}
