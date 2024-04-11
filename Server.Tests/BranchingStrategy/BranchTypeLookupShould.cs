using Microsoft.Extensions.Options;

namespace PrincipleStudios.ScaledGitApp.BranchingStrategy;

public class BranchTypeLookupShould
{
	private readonly BranchTypeLookup branchTypeLookup;

	public BranchTypeLookupShould()
	{
		var configuration = new BranchingStrategyConfiguration
		{
			DefaultBranchType = "feature",
			BranchTypes = new Dictionary<string, BranchTypeConfiguration>
			{
				["main"] = new()
				{
					Patterns = ["^main$"],
					Colors = ["rgb(112, 112, 112)"],
				},
				["master"] = new()
				{
					Patterns = ["^master$"],
					Colors = ["rgb(113, 113, 113)"],
				},
				["service-line"] = new()
				{
					Patterns = ["^line/"],
					Colors = ["rgb(115, 115, 115)"],
				},
				["feature"] = new()
				{
					Patterns = [],
					Colors = ["rgb(111, 111, 111)"],
				},
				["release-candidate"] = new()
				{
					Patterns = ["^rc/"],
					Colors = ["rgb(114, 114, 114)"],
				}
			},
		};
		branchTypeLookup = new BranchTypeLookup(configuration);
	}

	[InlineData("branch-not-in-config", "rgb(111, 111, 111)")]
	[InlineData("main", "rgb(112, 112, 112)")]
	[InlineData("master", "rgb(113, 113, 113)")]
	[InlineData("rc/24.4.1", "rgb(114, 114, 114)")]
	[InlineData("rca", "rgb(111, 111, 111)")]
	[InlineData("line/24.4.1", "rgb(115, 115, 115)")]
	[InlineData("lineman", "rgb(111, 111, 111)")]
	[Theory]
	public void Given_branch_name_return_expected_color(string branchName, string expectedColor)
	{
		var typeInfo = branchTypeLookup.DetermineBranchType(branchName);

		Assert.Equal(expectedColor, typeInfo.Color);
	}
}
