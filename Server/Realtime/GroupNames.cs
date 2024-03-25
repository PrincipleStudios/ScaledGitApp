using Microsoft.AspNetCore.SignalR;

namespace PrincipleStudios.ScaledGitApp.Realtime;

public static class GroupNames
{
	public const string AuthenticatedUser = "AuthenticatedUser";

	public static IClientProxy AuthenticatedUsers(this IHubClients clients)
	{
		return clients.Group(AuthenticatedUser);
	}
}
