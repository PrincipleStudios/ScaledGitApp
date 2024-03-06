namespace PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

public record BranchExists(string FullBranchName) : IPowerShellCommand<Task<bool>>
{
	public async Task<bool> RunCommand(IPowerShellCommandContext pwsh)
	{
		var branchExistenceCheck = await pwsh.InvokeCliAsync("git", ["rev-parse", "--verify", FullBranchName]);
		var branchExists = !branchExistenceCheck.HadErrors;
		return branchExists;
	}
}
