using PrincipleStudios.ScaledGitApp.Git.ToolsCommands;
using PrincipleStudios.ScaledGitApp.Realtime.Messages;

namespace PrincipleStudios.ScaledGitApp.Api.Git;

public class GitFetchController(
	IGitToolsCommandInvoker gitToolsPowerShell,
	IRealtimeMessageInvoker realtime
) : GitFetchControllerBase
{
	protected override async Task<RequestGitFetchActionResult> RequestGitFetch()
	{
		await gitToolsPowerShell.RunCommand(new GitFetch());
		await realtime.RunCommand(new GitFetchedMessage());
		return RequestGitFetchActionResult.Ok();
	}
}
