
namespace PrincipleStudios.ScaledGitApp.BranchingStrategy
{
	public interface IConflictLocator
	{
		Task<IReadOnlyList<IdentifiedConflict>> FindConflictsBetween(IReadOnlyList<string> leftBranches, IReadOnlyList<string> rightBranches);
		Task<IReadOnlyList<IdentifiedConflict>> FindConflictsWithin(IReadOnlyList<string> branches);
	}
}