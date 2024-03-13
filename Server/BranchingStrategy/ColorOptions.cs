namespace PrincipleStudios.ScaledGitApp.BranchingStrategy
{
	public record ColorOptions
	{
		public required string Default { get; init; }

		public required Dictionary<string, string> Branches { get; init; }

		public required Dictionary<string, string> BranchPrefixes { get; init; }
	}
}
