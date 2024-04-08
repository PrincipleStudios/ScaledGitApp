namespace PrincipleStudios.ScaledGitApp.BranchingStrategy;

public class BranchingStrategyConfiguration
{
	public required string DefaultBranchType { get; init; }
	public required Dictionary<string, BranchTypeConfiguration> BranchTypes { get; init; }
}

public class BranchTypeConfiguration
{
	public required string[] Patterns { get; init; }
	public required string[] Colors { get; init; }
}
