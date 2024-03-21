namespace PrincipleStudios.ScaledGitApp.Auth;

public static class ServiceRegistration
{
	internal static void RegisterAuth(this IServiceCollection services, IConfigurationSection configurationSection)
	{
		services.AddAuthorization(options =>
		{
			options.AddPolicy("AuthenticatedUser", builder =>
			{
				// We want config-based rules, but every policy must have a requirement.
				// This "always allowed" requirement satisfies that rule.
				builder.RequireAssertion(context => true);
			});
		});
	}
}
