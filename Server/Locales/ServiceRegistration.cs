using PrincipleStudios.ScaledGitApp.Git;

namespace PrincipleStudios.ScaledGitApp.Locales;

public static class ServiceRegistration
{
	internal static void RegisterLocales(this IServiceCollection services, IConfigurationSection configurationSection)
	{
		// Configuration
		services.Configure<LocalizationOptions>(configurationSection);

		// Services
		services.AddSingleton<ILocaleLoader, LocaleLoader>();
	}
}
