namespace PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

public record BranchExists(string FullBranchName) : IPowerShellCommand<Task<bool>>
{
	public async Task<bool> Execute(IPowerShellCommandContext context)
	{
		var branchExistenceCheck = await context.InvokeCliAsync("git", ["rev-parse", "--verify", FullBranchName]);
		var branchExists = !branchExistenceCheck.HadErrors;
		return branchExists;
	}
}
