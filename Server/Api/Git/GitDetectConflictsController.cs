using PrincipleStudios.ScaledGitApp.Api.Git.Conversions;
using PrincipleStudios.ScaledGitApp.BranchingStrategy;
using PrincipleStudios.ScaledGitApp.Git;
using PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

namespace PrincipleStudios.ScaledGitApp.Api.Git;

public class GitDetectConflictsController(IGitToolsCommandInvoker gitToolsPowerShell, IBranchTypeLookup branchTypeLookup, IBranchDetailsMapper branchDetailsMapper, IConflictLocator conflictLocator) : GitDetectConflictsControllerBase
{
	protected override async Task<GetConflictDetailsActionResult> GetConflictDetails(GetConflictDetailsRequest getConflictDetailsBody)
	{
		IReadOnlyList<string> branches = getConflictDetailsBody.Branches.Order().ToArray();
		if (branches.Count == 0)
			return GetConflictDetailsActionResult.BadRequest();

		var upstreams = await gitToolsPowerShell.RunCommand(new GitUpstreamData());
		// If only one branch was provided, we probably need to find conflicts with 
		if (branches is [string originalBranch] && upstreams.TryGetValue(originalBranch, out var originalUpstreams))
			branches = originalUpstreams.UpstreamBranchNames.Order().ToArray();
		if (branches.Count < 2)
			return GetConflictDetailsActionResult.BadRequest();

		var relevantBranches = GetRelevantBranches(branches, upstreams);

		// Check to see if updates need to be pulled for any of the relevant
		// branches before checking for conflicts. Not having everything
		// up-to-date can hide where the conflicts actually belong.
		var allBranchUpstreamDetails = await GetBranchDetails(relevantBranches);
		var outOfDate = allBranchUpstreamDetails.Where(b => b.Upstreams.Any(u => u.HasConflict || u.BehindCount > 0)).ToArray();
		if (outOfDate.Length > 0) return GetConflictDetailsActionResult.Conflict(outOfDate.Select(branchDetailsMapper.ToBranchDetails));

		// Determine if the named branches actually have conflicts
		var conflicts = await conflictLocator.FindConflictsWithin(branches);

		return GetConflictDetailsActionResult.Ok(conflicts.Select(ToConflictDetails));
	}

	/// <summary>
	/// Determine the upstream relevant branches - if there are fully-shared
	/// upstreams, they aren't relevant. (An infra branch shared by all branches
	/// specified won't cause new conflicts.)
	///
	/// Given the following (upstream->downstream) structure: [main->infra,
	/// infra->feature-1, infra->feature-2, feature-1->task-123,
	/// feature-1->task-124, feature-2->task-456]
	///
	/// If 'task-123', 'task-124', and 'task-456' are passed in, then the return
	/// results will be: ['feature-1', 'task-123', 'task-124', 'feature-2',
	/// 'task-456']
	/// </summary>
	private static IEnumerable<string> GetRelevantBranches(IReadOnlyList<string> branches, IReadOnlyDictionary<string, UpstreamBranchConfiguration> upstreams)
	{
		var commonUpstreams = upstreams.GetCommonUpstream(branches);
		return branches.Concat(branches.SelectMany(upstreams.GetAllUpstream)).Except(commonUpstreams).Distinct();
	}

	private async Task<UpstreamBranchDetailedState[]> GetBranchDetails(IEnumerable<string> branches)
	{
		var result = new List<UpstreamBranchDetailedState>();
		foreach (var current in branches)
		{
			var next = await gitToolsPowerShell.RunCommand(new GitBranchUpstreamDetails(current, IncludeDownstream: false, IncludeUpstream: false, Recurse: false));
			var state = next[0];
			result.Add(state);
		}

		return result.ToArray();
	}

	private ConflictDetails ToConflictDetails(IdentifiedConflict source)
	{
		return new ConflictDetails(
			Branches: new[] { source.Branches.LeftBranch, source.Branches.RightBranch }.Select(ToBranch),
			Files: source.ConflictingFiles.ConflictingFiles.Select((file) => ToFileDetails(file))
		);
	}

	private Branch ToBranch(string branchName)
	{
		var info = branchTypeLookup.DetermineBranchType(branchName);
		return new Branch(Name: branchName, Color: info.Color, Type: info.BranchType);
	}

	private static FileConflictDetails ToFileDetails(ScaledGitApp.Git.ToolsCommands.FileConflictDetails fileConflictDetails)
	{
		return new FileConflictDetails(Path: fileConflictDetails.FilePath);
	}
}
