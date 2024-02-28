using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.BranchingStrategy;

public static class ServiceRegistration
{
	internal static void RegisterBranchingStrategy(this IServiceCollection services)
	{
		services.AddSingleton<IColorConfiguration, ColorConfiguration>();
	}
}
