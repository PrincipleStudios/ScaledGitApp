using PrincipleStudios.ScaledGitApp.ShellUtilities;
using System.Text.RegularExpressions;

namespace PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

public class GetConflictingFiles(string LeftBranch, string RightBranch, IReadOnlyList<string>? HelpfulIntegrations = null) : IPowerShellCommand<Task<GetConflictingFilesResult>>
{
	// "stage" is a term used within git for merge information. From https://git-scm.com/docs/git-merge:
	// stage 1 stores the version from the common ancestor, stage 2 from HEAD, and stage 3 from MERGE_HEAD
	const string commonAncestorStage = "1";
	const string leftStage = "2";
	const string rightStage = "3";

	private record WriteTreeResult(
		bool HasConflict,
		string ResultTreeHash,
		IReadOnlyList<FileConflictDetails> ConflictingFiles,
		IReadOnlyList<ConflictRecord> ConflictMessages
	);

	// mode ' ' hash ' ' stage '\t' path
	private static readonly Regex fileInfoRegex = new(@"^(?<mode>[^ ]+) (?<hash>[^ ]+) (?<stage>[^\t]+)\t(?<path>.+)$");

	public async Task<GetConflictingFilesResult> Execute(IPowerShellCommandContext context)
	{
		var usedIntegrationBranches = new List<string>();
		var remainingIntegration = (HelpfulIntegrations ?? []).ToList();
		var currentLeft = LeftBranch;
		while (remainingIntegration.Count > 0)
		{
			var allFailed = true;
			for (var i = 0; i < remainingIntegration.Count; i++)
			{
				var next = await WriteTree(context, currentLeft, remainingIntegration[i]);
				if (next.HasConflict) continue;
				allFailed = false;
				currentLeft = await Commit(context, next.ResultTreeHash, [currentLeft, remainingIntegration[i]]);
				usedIntegrationBranches.Add(remainingIntegration[i]);
				remainingIntegration.Remove(remainingIntegration[i]);
				i--;
			}
			if (allFailed) break;
		}

		var result = await WriteTree(context, currentLeft, RightBranch);

		return new GetConflictingFilesResult(
			result.HasConflict,
			result.ResultTreeHash,
			result.ConflictingFiles,
			result.ConflictMessages,
			usedIntegrationBranches.Order().ToArray().AsReadOnly()
		);
	}

	private static async Task<WriteTreeResult> WriteTree(IPowerShellCommandContext context, string LeftBranch, string RightBranch)
	{
		var cliResults = await context.InvokeCliAsync("git", "merge-tree", "-z", "--write-tree", LeftBranch, RightBranch);
		var fullOutput = string.Join('\n', cliResults.ToResultStrings(allowErrors: true));
		if (string.IsNullOrEmpty(fullOutput)) throw GitException.From(cliResults);
		var entries = fullOutput.Split('\0').ToArray();
		// The entries will always end with a blank string since the response from git will end with a '\0'

		var treeHash = entries[0];
		var fileInfoSection = entries[1..Array.FindIndex(entries, 1, entries.Length - 1, string.IsNullOrEmpty)];

		var fileInfo = fileInfoSection
			.Select(line => fileInfoRegex.Match(line))
			.Select(match => !match.Success ? throw new InvalidOperationException("Unable to parse git merge-tree result") : match)
			.GroupBy(match => match.Groups["path"].Value)
			.Select(ToFileConflictDetails)
			.ToArray();

		var conflictMessages = new List<ConflictRecord>();
		for (var i = fileInfoSection.Length + 2; i < entries.Length - 1;)
		{
			var pathCount = int.Parse(entries[i]);
			var paths = entries[(i + 1)..(i + pathCount + 1)];
			conflictMessages.Add(new ConflictRecord(
				paths,
				ConflictType: entries[i + pathCount + 1],
				Message: entries[i + pathCount + 2].Trim()
			));
			i += pathCount + 3;
		}

		return new WriteTreeResult(
			HasConflict: cliResults.HadErrors,
			ResultTreeHash: treeHash,
			ConflictingFiles: fileInfo,
			ConflictMessages: conflictMessages.ToArray()
		);
	}

	private static async Task<string> Commit(IPowerShellCommandContext context, string treeish, IReadOnlyList<string> commitishes)
	{
		var args = new[] { "commit-tree", treeish, "-m", "interim merge" }.Concat(commitishes.SelectMany(commitish => new[] { "-p", commitish })).ToArray();
		var cliResults = await context.InvokeCliAsync("git", args);
		return cliResults.ToResultStrings(allowErrors: false).Single();
	}

	private static FileConflictDetails ToFileConflictDetails(IGrouping<string, Match> grouping)
	{
		return new FileConflictDetails(
			FilePath: grouping.Key,
			MergeBase: ToGitFileInfoSafe(grouping, commonAncestorStage),
			Left: ToGitFileInfoSafe(grouping, leftStage),
			Right: ToGitFileInfoSafe(grouping, rightStage)
		);
	}

	private static GitFileInfo? ToGitFileInfoSafe(IGrouping<string, Match> grouping, string stage)
	{
		return grouping.SingleOrDefault(m => m.Groups["stage"].Value == stage) is Match match
			? ToGitFileInfo(match)
			: null;
	}

	private static GitFileInfo ToGitFileInfo(Match match)
	{
		return new GitFileInfo(match.Groups["mode"].Value, match.Groups["hash"].Value);
	}
}

public record GitFileInfo(string Mode, string Hash);
public record FileConflictDetails(string FilePath, GitFileInfo? MergeBase, GitFileInfo? Left, GitFileInfo? Right);
public record ConflictRecord(IReadOnlyList<string> FilePaths, string ConflictType, string Message);
public record GetConflictingFilesResult(
	bool HasConflict,
	string ResultTreeHash,
	IReadOnlyList<FileConflictDetails> ConflictingFiles,
	IReadOnlyList<ConflictRecord> ConflictMessages,
	IReadOnlyList<string> IncludedIntegrationBranches)
{
	public static GetConflictingFilesResult Empty(string hash, IReadOnlyList<string> includedIntegrationBranches) => new GetConflictingFilesResult(
		HasConflict: false,
		ResultTreeHash: hash,
		ConflictingFiles: [],
		ConflictMessages: [],
		IncludedIntegrationBranches: includedIntegrationBranches
	);
}
