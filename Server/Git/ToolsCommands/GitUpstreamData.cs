using Microsoft.Extensions.Options;
using System.Collections.Immutable;
using System.Management.Automation;
using System.Text.RegularExpressions;

namespace PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

public record GitUpstreamData() : IGitToolsCommand<Task<IReadOnlyDictionary<string, UpstreamBranchConfiguration>>>
{
	private static readonly Regex gitLsTreeRecursiveLine = new Regex(@"^(?<hash>[A-Za-z0-9]{40}) (?<name>[^\t]+)$");

	public async Task<IReadOnlyDictionary<string, UpstreamBranchConfiguration>> RunCommand(IGitToolsCommandContext pwsh)
	{
		var result = await pwsh.InvokeCliAsync("git", "ls-tree", "-r", pwsh.GitCloneConfiguration.UpstreamBranchName, "--format=%(objectname) %(path)");
		if (result.HadErrors) return ImmutableDictionary<string, UpstreamBranchConfiguration>.Empty;

		var branches = (
			from entry in result.Results
			let match = gitLsTreeRecursiveLine.Match(entry.ToString())
			where match.Success
			select new { Hash = match.Groups["hash"].Value, Name = match.Groups["name"].Value }
		).ToDictionary(e => e.Name, e => e.Hash);

		using var hashes = new PSDataCollection<string>(branches.Values.Distinct());
		if (hashes.Count == 0)
			return ImmutableDictionary<string, UpstreamBranchConfiguration>.Empty;
		var hashEntries = await pwsh.InvokeCliAsync("git", arguments: ["cat-file", "--batch=\t%(objectname)"], input: hashes);
		var actualEntries = (
			from e in string.Join('\n', hashEntries.Results.Select(r => r.ToString())).Split('\t')
			where !string.IsNullOrWhiteSpace(e)
			let hash = e.Split('\n')
			select new { hash = hash[0], branches = hash.Skip(1).Where(b => !string.IsNullOrWhiteSpace(b)).ToArray() }
		).ToDictionary(e => e.hash, e => e.branches);

		return branches.ToDictionary(b => b.Key, b => new UpstreamBranchConfiguration(actualEntries[b.Value]));
	}
}

public record UpstreamBranchConfiguration(IReadOnlyList<string> UpstreamBranchNames);
