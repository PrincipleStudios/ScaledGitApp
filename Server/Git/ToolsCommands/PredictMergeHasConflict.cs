using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

public record GetConflictingFiles(string LeftBranch, string RightBranch) : IGitToolsCommand<Task<bool>>
{
	public async Task<bool> RunCommand(IGitToolsCommandContext pwsh)
	{
		var cliResults = await pwsh.InvokeCliAsync("git", "merge-tree", "--name-only", "--no-messages", LeftBranch, RightBranch);
		return cliResults.HadErrors;
	}
}
