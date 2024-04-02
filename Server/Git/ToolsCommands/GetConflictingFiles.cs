using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

public record GetConflictingFiles(string LeftBranch, string RightBranch) : IPowerShellCommand<Task<GetConflictingFilesResult>>
{
	public async Task<GetConflictingFilesResult> RunCommand(IPowerShellCommandContext context)
	{
		var cliResults = await context.InvokeCliAsync("git", "merge-tree", "--name-only", "--no-messages", LeftBranch, RightBranch);
		var entries = cliResults.ToResultStrings(allowErrors: true).ToList();
		if (entries.Count == 0) throw GitException.From(cliResults);

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
