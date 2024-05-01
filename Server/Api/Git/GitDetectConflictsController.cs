using PrincipleStudios.ScaledGitApp.BranchingStrategy;
using PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

namespace PrincipleStudios.ScaledGitApp.Api.Git;

public class GitDetectConflictsController(IGitToolsCommandInvoker gitToolsPowerShell, IBranchTypeLookup branchTypeLookup) : GitDetectConflictsControllerBase
{
	protected override async Task<GetConflictDetailsActionResult> GetConflictDetails(GetConflictDetailsRequest getConflictDetailsBody)
	{
		var branches = getConflictDetailsBody.Branches.ToArray();
		if (branches.Length != 2)
			return GetConflictDetailsActionResult.Unsafe(BadRequest());

		// TODO: check to see if updates need to be pulled for any of the provided branches before checking for conflicts
		// TODO: map the branches passed in to the full path or fix the command

		var conflictDetails = await GetConflictDetails(branches[0], branches[1]);
		if (conflictDetails == null) return GetConflictDetailsActionResult.Ok(Enumerable.Empty<ConflictDetails>());

		// TODO: Naive implementation; needs to check upstreams
		return GetConflictDetailsActionResult.Ok([conflictDetails]);
	}

	private async Task<ConflictDetails?> GetConflictDetails(string leftBranch, string rightBranch)
	{
		var result = await gitToolsPowerShell.RunCommand(new GetConflictingFiles(leftBranch, rightBranch));
		if (!result.HasConflict) return null;


		return new ConflictDetails(new[] { leftBranch, rightBranch }.Select(ToBranch), result.ConflictingFileNames.Select(ToFileDetails));
	}

	private Branch ToBranch(string branchName)
	{
		var info = branchTypeLookup.DetermineBranchType(branchName);
		return new Branch(Name: branchName, Color: info.Color, Type: info.BranchType);
	}

	private FileConflictDetails ToFileDetails(string path)
	{
		return new FileConflictDetails(Path: path);
	}
}
