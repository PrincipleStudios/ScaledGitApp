namespace PrincipleStudios.ScaledGitApp.Api.Environment;

public static class ServiceRegistration
{
	internal static void RegisterEnvironment(this IServiceCollection services, IConfigurationSection configurationSection)
	{
		services.AddHealthChecks();
		services.AddControllers();

		services.Configure<BuildOptions>(configurationSection);

		services.AddSpaStaticFiles(configuration =>
		{
			configuration.RootPath = "wwwroot";
		});
	}
}
