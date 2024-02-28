namespace PrincipleStudios.ScaledGitApp.BranchingStrategy;

public class ColorConfiguration : IColorConfiguration
{
	public string DetermineColor(string branchName)
	{
		// TODO: move to somewhere with configuration
		if (branchName.StartsWith("rc/")) return "rgb(111, 37, 111)";
		if (branchName.StartsWith("line/") || branchName == "main" || branchName == "master") return "rgb(111, 206, 31)";
		if (branchName.StartsWith("integ/") || branchName.StartsWith("integrate/") || branchName.StartsWith("integration/")) return "rgb(98, 98, 98)";
		return "rgb(55, 127, 192)";
	}
}
