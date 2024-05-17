using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.BranchingStrategy;

public static class ServiceRegistration
{
	internal static void RegisterBranchingStrategy(this IServiceCollection services, IConfigurationSection strategyOptionsConfig)
	{
		services.Configure<BranchingStrategyOptions>(strategyOptionsConfig);
		services.AddSingleton<IBranchTypeLookup, BranchTypeLookup>();
		services.AddSingleton<IConflictLocator, ConflictLocator>();
	}
}
