using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

public class GitGetObject(string objectish) : IPowerShellCommand<Task<string>>
{
	public async Task<string> Execute(IPowerShellCommandContext context)
	{
		// TODO: What happens if the requested file is binary? Or very large?
		var cliResults = await context.InvokeCliAsync("git", "cat-file", "-p", objectish);
		return string.Join('\n', cliResults.ToResultStrings());
	}
}
