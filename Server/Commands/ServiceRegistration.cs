namespace PrincipleStudios.ScaledGitApp.Commands;

public static class ServiceRegistration
{
	internal static void RegisterCommands(this IServiceCollection services, IConfigurationSection commandConfig)
	{
		services.Configure<CommandCacheOptions>(commandConfig.GetSection("Cache"));
		services.AddSingleton<ICommandCache, CommandCache>();
		services.AddMemoryCache(options =>
		{
		});
	}
}
