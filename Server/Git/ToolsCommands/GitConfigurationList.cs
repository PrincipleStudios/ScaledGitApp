using PrincipleStudios.ScaledGitApp.ShellUtilities;
using System.Collections.Immutable;

namespace PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

public class GitConfigurationList : IPowerShellCommand<Task<IReadOnlyDictionary<string, IReadOnlyList<string>>>>
{
	public async Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> RunCommand(IPowerShellCommandContext context)
	{
		var configurationResult = await context.InvokeCliAsync("git", "config", "--list");
		configurationResult.ThrowIfHadErrors();
		return
			(
				from configurationLine in configurationResult.ToResultStrings()
				let parts = configurationLine.Split('=', 2)
				group parts[1] by parts[0]
			).ToImmutableDictionary(g => g.Key, g => g.ToArray() as IReadOnlyList<string>, StringComparer.InvariantCultureIgnoreCase);
	}
}
