using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

public record GetCommitCount(IEnumerable<string> Included, IEnumerable<string> Excluded) : IGitToolsCommand<Task<int?>>
{
	private static readonly IReadOnlyList<string> countArgs = ["rev-list", "--count"];

	public async Task<int?> RunCommand(IGitToolsPowerShellCommandContext pwsh)
	{
		var cliResults = await pwsh.InvokeCliAsync("git", countArgs.Concat(Included).Concat(Excluded.Select(b => $"^{b}")).ToArray());
		if (cliResults.HadErrors) return null;
		return int.Parse(cliResults.ToResultStrings().Single());
	}
}
