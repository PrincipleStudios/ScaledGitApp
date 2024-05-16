
using PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

namespace PrincipleStudios.ScaledGitApp.Api.Git;

public class GitRetrieveObjectController(
	IGitToolsCommandInvoker gitToolsPowerShell
) : GitRetrieveObjectControllerBase
{
	// 512kb is a pretty large file...
	const int maxReturnedFileSize = 512 * 1024;

	protected override async Task<RetrieveGitObjectActionResult> RetrieveGitObject(string objectish)
	{
		try
		{
			var result = await gitToolsPowerShell.RunCommand(new GitGetObject(objectish));

			// TODO: this still requires loading the full file in memory before returning it; this could be dangerous for memory usage.
			if (result.Length > maxReturnedFileSize) return RetrieveGitObjectActionResult.PartialContent(result.Substring(0, maxReturnedFileSize));
			return RetrieveGitObjectActionResult.Ok(result);
		}
		catch
		{
			return RetrieveGitObjectActionResult.NotFound();
		}
	}
}
