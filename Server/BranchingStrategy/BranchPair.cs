namespace PrincipleStudios.ScaledGitApp.BranchingStrategy;

public class BranchPair : BranchSet
{
	public string LeftBranch => Branches[0];
	public string RightBranch => Branches[1];

	public BranchPair(string leftBranch, string rightBranch) : base(leftBranch, rightBranch)
	{
	}

	public string OtherBranch(string branch) =>
		branch switch
		{
			_ when branch == LeftBranch => RightBranch,
			_ when branch == RightBranch => LeftBranch,
			_ => throw new ArgumentException("Neither left nor right branch was provided")
		};
}
