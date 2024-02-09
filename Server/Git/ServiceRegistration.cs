
namespace PrincipleStudios.ScaledGitApp.Git;

public static class ServiceRegistration
{
	internal static void RegisterGit(this IServiceCollection services, IConfigurationSection configurationSection)
	{
		services.Configure<GitOptions>(configurationSection);
		services.AddHostedService<GitCloneService>();
	}
}
