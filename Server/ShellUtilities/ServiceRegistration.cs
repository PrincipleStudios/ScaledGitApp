using PrincipleStudios.ScaledGitApp.Git;

namespace PrincipleStudios.ScaledGitApp.ShellUtilities;

public static class ServiceRegistration
{
	internal static void RegisterShellUtilities(this IServiceCollection services)
	{
		services.AddSingleton<PowerShellFactory>();
	}
}
