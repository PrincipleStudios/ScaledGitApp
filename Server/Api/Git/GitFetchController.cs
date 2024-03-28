using PrincipleStudios.ScaledGitApp.Commands;
using PrincipleStudios.ScaledGitApp.Git.ToolsCommands;
using PrincipleStudios.ScaledGitApp.Realtime.Messages;

namespace PrincipleStudios.ScaledGitApp.Api.Git;

public class GitFetchController(
	IGitToolsCommandInvoker gitToolsPowerShell,
	IRealtimeMessageInvoker realtime,
	ICommandCache commandCache
) : GitFetchControllerBase
{
	protected override async Task<RequestGitFetchActionResult> RequestGitFetch()
	{
		await gitToolsPowerShell.RunCommand(new GitFetch());
		commandCache.ClearAll();
		await realtime.RunCommand(new GitFetchedMessage());
		return RequestGitFetchActionResult.Ok();
	}
}
