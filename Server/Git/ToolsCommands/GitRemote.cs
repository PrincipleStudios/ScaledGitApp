using PrincipleStudios.ScaledGitApp.ShellUtilities;
using System.Text.RegularExpressions;

namespace PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

/// <summary>
/// Clones a git repository into the git working directory.
/// </summary>
/// <param name="Repository">The Git URL for the repository to clone</param>
public record GitRemote() : IPowerShellCommand<Task<GitRemoteResult>>
{
	private static readonly Regex gitRemoteLine = new Regex(@"^(?<alias>[^\t]+)\t(?<url>[^ ]+) \(fetch\)$");
	public async Task<GitRemoteResult> RunCommand(IPowerShellCommandContext context)
	{
		var result = (await context.InvokeCliAsync("git", "remote", "-v")).ToResultStrings();

		return new GitRemoteResult((
			from line in result
			let match = gitRemoteLine.Match(line)
			where match.Success
			select new GitRemoteEntry(match.Groups["alias"].Value, match.Groups["url"].Value)
		).ToArray());
	}
}

public record GitRemoteResult(IReadOnlyList<GitRemoteEntry> Remotes);
public record GitRemoteEntry(string Alias, string FetchUrl);
