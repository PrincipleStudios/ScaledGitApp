using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using PrincipleStudios.ScaledGitApp.Environment;

namespace PrincipleStudios.ScaledGitApp.Realtime;

[Microsoft.AspNetCore.Authorization.Authorize(Policy = "AuthenticatedUser")]
public class FullHub(IOptions<BuildOptions> buildOptions) : Hub
{
	public override async Task OnConnectedAsync()
	{
		if (Context.User?.Identity == null)
		{
			// Shouldn't happen due to Authorize policy above
			Context.Abort();
			return;
		}

		await Clients.Caller.SendAsync("GitHash", buildOptions.Value.GitHash);

		await Groups.AddToGroupAsync(Context.ConnectionId, GroupNames.AuthenticatedUser);
	}
}
