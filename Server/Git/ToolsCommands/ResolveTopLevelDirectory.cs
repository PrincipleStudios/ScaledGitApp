using Microsoft.Extensions.Options;
using PrincipleStudios.ScaledGitApp.ShellUtilities;
using System.Management.Automation;

namespace PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

public class ResolveTopLevelDirectory() : IPowerShellCommand<Task<string>>
{
	public async Task<string> Execute(IPowerShellCommandContext context)
	{
		// Gets the _actual_ top level of the working directory
		var result = await context.InvokeCliAsync("git", "rev-parse", "--show-toplevel");
		result.ThrowIfHadErrors();
		return result switch
		{
			{ HadErrors: false, Results: [PSObject item] } => item.ToString(),
			_ => throw new InvalidOperationException("Unknown result from `git rev-parse --show-toplevel`")
		};
	}
}
