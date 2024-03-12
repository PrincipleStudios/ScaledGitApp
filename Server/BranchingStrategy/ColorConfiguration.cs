using Microsoft.Extensions.Options;

namespace PrincipleStudios.ScaledGitApp.BranchingStrategy;

public class ColorConfiguration(IOptions<ColorOptions> colorOptions) : IColorConfiguration
{
	private readonly ColorOptions colorOptions = colorOptions.Value;

	public string DetermineColor(string branchName)
	{
		if (colorOptions.Branches.TryGetValue(branchName, out string? value))
			return value;

		return colorOptions.BranchPrefixes.FirstOrDefault(bp => branchName.StartsWith(bp.Key)).Value
			?? colorOptions.Default;
	}
}
