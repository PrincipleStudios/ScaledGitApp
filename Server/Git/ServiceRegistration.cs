
using Microsoft.Extensions.Options;

namespace PrincipleStudios.ScaledGitApp.Git;

public static class ServiceRegistration
{
	internal static void RegisterGit(this IServiceCollection services, IConfigurationSection configurationSection)
	{
		// Apps that will run on their own
		services.AddHostedService<GitCloneService>();

		// Configuration
		services.Configure<GitOptions>(configurationSection);

		// Services
		services.AddTransient<GitCloneConfigurationFactory>();
		services.AddSingleton<IGitToolsInvoker>(sp =>
		{
			Func<Task<GitCloneConfiguration>> factory = sp.GetRequiredService<GitCloneConfigurationFactory>().DetectGitCloneConfiguration;
			return ActivatorUtilities.CreateInstance<GitToolsPowerShellInvoker>(sp, factory);
		});
	}
}
