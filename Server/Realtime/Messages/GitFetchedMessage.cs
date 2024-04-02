using PrincipleStudios.ScaledGitApp.Commands;

namespace PrincipleStudios.ScaledGitApp.Realtime.Messages;

public class GitFetchedMessage : ICommand<Task, IRealtimeMessageContext>
{
	private const string gitFetchedMessage = "GitFetched";

	public Task RunCommand(IRealtimeMessageContext context)
	{
		return context.HubContext.Clients.AuthenticatedUsers().SendCoreAsync(gitFetchedMessage, []);
	}
}
