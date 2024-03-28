namespace PrincipleStudios.ScaledGitApp.Commands;

public static class ServiceRegistration
{
	internal static void RegisterCommands(this IServiceCollection services)
	{
		services.AddSingleton<ICommandCache, CommandCache>();
		services.AddMemoryCache(options =>
		{
		});
	}
}
