namespace PrincipleStudios.ScaledGitApp.BranchingStrategy;

public interface IBranchTypeLookup
{
	BranchTypeInfo DetermineBranchType(string branchName);
}

public record BranchTypeInfo(string BranchType, string Color);
