using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace PrincipleStudios.ScaledGitApp.BranchingStrategy;

public class BranchTypeLookup(BranchingStrategyConfiguration configuration) : IBranchTypeLookup
{
	private ConcurrentDictionary<string, IReadOnlyList<Regex>> patterns = new ConcurrentDictionary<string, IReadOnlyList<Regex>>();
	public BranchTypeLookup(IOptions<BranchingStrategyOptions> strategyOptions)
		: this(LoadBranchingStrategyConfiguration(strategyOptions.Value.PathToBranchingStrategy))
	{
	}

	public BranchTypeInfo DetermineBranchType(string branchName)
	{
		var type = FindBranchTypeName(branchName);
		var colors = configuration.BranchTypes[type].Colors;
		return new(type, GetColor(colors, branchName));
	}

	private static string GetColor(string[] colors, string branchName)
	{
		switch (colors)
		{
			case []:
				return "rgb(55, 127, 192)";
			case [var color]:
				return color;
			default:
				return colors[Math.Abs(branchName.GetHashCode() % colors.Length)];
		}
	}

	private string FindBranchTypeName(string branchName)
	{
		return configuration.BranchTypes.Keys
			.FirstOrDefault(
				branchType =>
					patterns.GetOrAdd(branchType, GetBranchTypePatterns).Any(regex => regex.IsMatch(branchName))
			)
			?? configuration.DefaultBranchType;
	}

	private IReadOnlyList<Regex> GetBranchTypePatterns(string branchType)
	{
		return configuration.BranchTypes[branchType].Patterns.Select(p => new Regex(p)).ToArray();
	}

	private static BranchingStrategyConfiguration LoadBranchingStrategyConfiguration(string? pathToBranchingStrategy = null)
	{
		using var stream = pathToBranchingStrategy == null
			? typeof(BranchTypeConfiguration).Assembly.GetManifestResourceStream("PrincipleStudios.ScaledGitApp.branching-strategy.yaml")
			: File.OpenRead(pathToBranchingStrategy);
		if (stream == null) throw new InvalidOperationException("Unable to load branching strategy configuration.");
		var serializer = new SharpYaml.Serialization.Serializer(new SharpYaml.Serialization.SerializerSettings
		{
			NamingConvention = new SharpYaml.Serialization.CamelCaseNamingConvention(),
		});
		var result = serializer.Deserialize<BranchingStrategyConfiguration>(stream);
		if (result == null) throw new InvalidOperationException("Invalid branching strategy configuration.");
		return result;
	}
}
