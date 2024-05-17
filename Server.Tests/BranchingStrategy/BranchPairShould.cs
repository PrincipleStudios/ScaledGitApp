namespace PrincipleStudios.ScaledGitApp.BranchingStrategy;

public class BranchPairShould
{
	[Theory]
	[InlineData("feature/a", "feature/b")]
	[InlineData("rc/2048", "feature/b")]
	public void Match_equality_when_parameters_are_transposed(string branch1, string branch2)
	{
		var first = new BranchPair(branch1, branch2);
		var second = new BranchPair(branch2, branch1);

		Assert.Equal(first, second);
	}

	[Theory]
	[InlineData("feature/a", "feature/b")]
	[InlineData("rc/2048", "feature/b")]
	public void Produce_identical_hash_codes_when_parameters_are_transposed(string branch1, string branch2)
	{
		var first = new BranchPair(branch1, branch2);
		var second = new BranchPair(branch2, branch1);

		Assert.Equal(first.GetHashCode(), second.GetHashCode());
	}

	[Theory]
	[InlineData("feature/a", "feature/b")]
	[InlineData("rc/2048", "feature/b")]
	public void Identify_other_branch(string branch1, string branch2)
	{
		var actual = new BranchPair(branch1, branch2);

		Assert.Equal(branch1, actual.OtherBranch(branch2));
		Assert.Equal(branch2, actual.OtherBranch(branch1));

		Assert.Throws<ArgumentException>(() => actual.OtherBranch("other-branch"));
	}
}
