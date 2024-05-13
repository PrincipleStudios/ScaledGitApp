namespace PrincipleStudios.ScaledGitApp.BranchingStrategy;

public class BranchPair
{
	public string LeftBranch { get; }
	public string RightBranch { get; }

	public BranchPair(string leftBranch, string rightBranch)
	{
		LeftBranch = leftBranch;
		RightBranch = rightBranch;
	}

	public override bool Equals(object? obj)
	{
		if (obj == null) return false;
		if (ReferenceEquals(this, obj)) return true;
		if (obj.GetType() != GetType()) return false;
		if (obj is not BranchPair other) return false;

		if (other.LeftBranch == LeftBranch && other.RightBranch == RightBranch) return true;
		if (other.LeftBranch == RightBranch && other.RightBranch == LeftBranch) return true;

		return false;
	}

	public override int GetHashCode()
	{
		return LeftBranch.GetHashCode() ^ RightBranch.GetHashCode();
	}

	public bool Includes(string branch) =>
		LeftBranch == branch || RightBranch == branch;

	public string OtherBranch(string branch) =>
		branch switch
		{
			_ when branch == LeftBranch => RightBranch,
			_ when branch == RightBranch => LeftBranch,
			_ => throw new ArgumentException("Neither left nor right branch was provided")
		};
}
