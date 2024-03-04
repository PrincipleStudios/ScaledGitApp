using Microsoft.Extensions.Options;
using PrincipleStudios.ScaledGitApp.ShellUtilities;
using System.Management.Automation;

namespace PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

public class ResolveTopLevelDirectory(string absoluteWorkingDirectory) : IPowerShellCommand<Task<string>>
{
	public async Task<string> RunCommand(IPowerShellCommandContext pwsh)
	{
		pwsh.SetCurrentWorkingDirectory(absoluteWorkingDirectory);
		// Gets the _actual_ top level of the working directory, in case 
		var gitTopLevel = await pwsh.InvokeCliAsync("git", "rev-parse", "--show-toplevel") switch
		{
			{ HadErrors: false, Results: [PSObject item] } => item.ToString(),
			{ HadErrors: true } => absoluteWorkingDirectory,
			_ => throw new InvalidOperationException("Unknown result from `git rev-parse --show-toplevel`")
		};
		return gitTopLevel;
	}
}
