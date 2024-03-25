namespace PrincipleStudios.ScaledGitApp.Realtime;

public static class ServiceRegistration
{
	internal static void RegisterRealtimeNotifications(this IServiceCollection services, bool includeAzureSignalR)
	{
		var signalr = services.AddSignalR();
#if IncludeAzure
		if (includeAzureSignalR)
			signalr.AddAzureSignalR();
#endif
	}
}
