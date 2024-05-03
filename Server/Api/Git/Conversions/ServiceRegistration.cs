namespace PrincipleStudios.ScaledGitApp.Api.Git.Conversions;

public static class ServiceRegistration
{
	internal static void RegisterGitConversions(this IServiceCollection services)
	{
		services.AddSingleton<IBranchDetailsMapper, BranchDetailsMapper>();
	}
}
