using PrincipleStudios.ScaledGitApp.Git;

namespace PrincipleStudios.ScaledGitApp.ShellUtililties;

public static class ServiceRegistration
{
	internal static void RegisterShellUtilities(this IServiceCollection services)
	{
		services.AddSingleton<PowerShellFactory>();
	}
}
