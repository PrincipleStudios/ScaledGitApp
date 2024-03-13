using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.BranchingStrategy;

public static class ServiceRegistration
{
	internal static void RegisterBranchingStrategy(this IServiceCollection services, IConfigurationSection colorConfig)
	{
		services.Configure<ColorOptions>(colorConfig);
		services.AddSingleton<IColorConfiguration, ColorConfiguration>();
	}
}
