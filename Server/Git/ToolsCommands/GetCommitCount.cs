﻿using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

public record GetCommitCount(IEnumerable<string> Included, IEnumerable<string> Excluded) : IPowerShellCommand<Task<int?>>
{
	private static readonly IReadOnlyList<string> countArgs = ["rev-list", "--count", "--no-merges"];

	public async Task<int?> RunCommand(IPowerShellCommandContext pwsh)
	{
		var cliResults = await pwsh.InvokeCliAsync("git", countArgs.Concat(Included).Concat(Excluded.Select(b => $"^{b}")).ToArray());
		if (cliResults.HadErrors) return null;
		return int.Parse(cliResults.ToResultStrings().Single());
	}
}
