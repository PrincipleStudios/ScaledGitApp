using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

public record GetConflictingFiles(string LeftBranch, string RightBranch) : IPowerShellCommand<Task<GetConflictingFilesResult>>
{
	public async Task<GetConflictingFilesResult> RunCommand(IPowerShellCommandContext pwsh)
	{
		var cliResults = await pwsh.InvokeCliAsync("git", "merge-tree", "--name-only", "--no-messages", LeftBranch, RightBranch);
		var entries = cliResults.ToResultStrings(allowErrors: true).ToList();
		var treeHash = entries[0];
		entries.RemoveAt(0);

		return new GetConflictingFilesResult(
			cliResults.HadErrors,
			treeHash,
			entries
		);
	}
}

public record GetConflictingFilesResult(bool HasConflict, string ResultTreeHash, IReadOnlyList<string> ConflictingFileNames);
