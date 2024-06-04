namespace PrincipleStudios.ScaledGitApp.BranchingStrategy;

public class BranchSet
{
	public IReadOnlyList<string> Branches { get; }

	public BranchSet(IEnumerable<string> branches)
	{
		Branches = branches.Distinct().ToArray().AsReadOnly();
	}

	public BranchSet(params string[] branches) : this((IEnumerable<string>)branches)
	{
	}

	public override bool Equals(object? obj)
	{
		if (obj == null) return false;
		if (ReferenceEquals(this, obj)) return true;
		if (obj.GetType() != GetType()) return false;
		if (obj is not BranchSet other) return false;

		if (other.Branches.Count != Branches.Count) return false;
		if (other.Branches.Except(Branches).Any()) return false;

		return true;
	}

	public override int GetHashCode()
	{
		return Branches.Count == 0
			? typeof(BranchSet).GetHashCode()
			: Branches.Select(b => b.GetHashCode()).Aggregate((prev, next) => prev & next);
	}

	public bool Includes(string branch) =>
		Branches.Contains(branch);

	public static readonly BranchSet Empty = new BranchSet(Enumerable.Empty<string>());
}
