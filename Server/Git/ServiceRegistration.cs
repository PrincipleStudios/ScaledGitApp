namespace PrincipleStudios.ScaledGitApp.Git;

public static class ServiceRegistration
{
	internal static void RegisterGit(this IServiceCollection services, IConfigurationSection configurationSection)
	{
		// Apps that will run on their own
		// AddHostedService doesn't check for a configured service for the hosted service
		services.AddHostedService(sp => sp.GetRequiredService<GitCloneService>());

		// Configuration
		services.Configure<GitOptions>(configurationSection);

		// Services
		services.AddSingleton<GitCloneService>();
		services.AddSingleton<IGitToolsCommandInvoker>(sp =>
		{
			var factory = () => sp.GetRequiredService<GitCloneService>().DetectedConfigurationTask;
			return ActivatorUtilities.CreateInstance<GitToolsCommandInvoker>(sp, factory);
		});
		services.AddSingleton<IPowerShellCommandInvoker, PowerShellCommandInvoker>();
	}
}
