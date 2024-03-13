using Microsoft.Extensions.Options;

namespace PrincipleStudios.ScaledGitApp.BranchingStrategy;

public class ColorConfigurationShould
{
	private readonly ColorConfiguration colorConfiguration;

	public ColorConfigurationShould()
	{
		var colorOptions = new ColorOptions
		{
			Default = "rgb(111, 111, 111)",
			Branches = new Dictionary<string, string>
			{
				{ "main", "rgb(112, 112, 112)" },
				{ "master", "rgb(113, 113, 113)" }
			},
			BranchPrefixes = new Dictionary<string, string>
			{
				{ "rc/", "rgb(114, 114, 114)" },
				{ "line/", "rgb(115, 115, 115)" },
			}
		};
		colorConfiguration = new ColorConfiguration(Options.Create(colorOptions));
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
		var color = colorConfiguration.DetermineColor(branchName);

		Assert.Equal(expectedColor, color);
	}
}
