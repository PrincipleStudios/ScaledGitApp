using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

/// <summary>
/// Clones a git repository into the git working directory.
/// </summary>
/// <param name="Repository">The Git URL for the repository to clone</param>
public record GitClone(string Repository) : IPowerShellCommand<Task>
{
	public async Task Execute(IPowerShellCommandContext context)
	{
		// TODO: git tools do not support bare repos, otherwise this should be a bare repo
		(await context.InvokeCliAsync("git", "clone", Repository, ".", "--quiet", "--no-checkout")).ThrowIfHadErrors();

		// Because this is not a bare repo, we need to go to a fake branch to basically simulate a bare repo.
		// This only affects performance.
		var currentBranch = (await context.InvokeCliAsync("git", "rev-parse", "--abbrev-ref", "HEAD")).ToResultStrings().Single();
		(await context.InvokeCliAsync("git", "checkout", "--orphan", "__fake", "--quiet")).ThrowIfHadErrors();
		(await context.InvokeCliAsync("git", "rm", "-rf", ".")).ThrowIfHadErrors();
		(await context.InvokeCliAsync("git", "branch", "-D", currentBranch)).ThrowIfHadErrors();
	}
}
