using Moq;
using Moq.Language.Flow;

namespace PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

public class GitBranchUpstreamDetailsShould
{
	private readonly GitToolsFixture fixture = new();
	// TODO: replace with dynamic branch names
	private readonly string baseBranchName = "feature/PS-123";
	private readonly string infraBranchName = "infra/ratchet-tests";
	private readonly string parentFeatureBranchName = "feature/PS-100";
	private readonly string mainBranchName = "main";

	private readonly GitBranchUpstreamDetails defaultValue;
	private readonly Dictionary<string, UpstreamBranchConfiguration> defaultUpstreamData;
	public GitBranchUpstreamDetailsShould()
	{
		defaultValue = new GitBranchUpstreamDetails(
			baseBranchName,
			IncludeDownstream: false,
			IncludeUpstream: false,
			Recurse: false
		);
		defaultUpstreamData = new()
		{
			{ baseBranchName, new ([ infraBranchName, parentFeatureBranchName ]) },
			{ infraBranchName, new ([ mainBranchName ]) },
			{ parentFeatureBranchName, new ([ infraBranchName ]) },
		};
		fixture.GitToolsCommandInvoker.Setup(i => i.RunCommand(It.IsAny<GitUpstreamData>())).ReturnsAsync(defaultUpstreamData);
	}

	private string ToFullName(string branchName) => fixture.CloneConfiguration.ToLocalTrackingBranchName(branchName)!;

	[Fact]
	public async Task Indicate_the_number_of_commits_the_current_branch_has_beyond_upstreams()
	{
		var target = defaultValue;
		SetupBaseBranchDetails(0, 0, 15);

		var branches = await target.Execute(fixture.Create());

		var actual = Assert.Single(branches);
		Assert.Equal(baseBranchName, actual.Name);
		Assert.Equal(15, actual.NonMergeCommitCount);
	}

	[Fact]
	public async Task Report_number_of_commits_missing_from_upstreams()
	{
		var target = defaultValue;
		SetupBaseBranchDetails(incomingFromInfra: 2, incomingFromParent: 5, additionalCommits: 0);

		var branches = await target.Execute(fixture.Create());

		var actual = Assert.Single(branches);
		Assert.Equal(baseBranchName, actual.Name);
		var infraBranchStatus = Assert.Single(actual.Upstreams, upstream => upstream.Name == infraBranchName);
		Assert.Equal(2, infraBranchStatus.BehindCount);
		var parentFeatureBranchStatus = Assert.Single(actual.Upstreams, upstream => upstream.Name == parentFeatureBranchName);
		Assert.Equal(5, parentFeatureBranchStatus.BehindCount);
	}

	[Fact]
	public async Task Report_downstreams_of_target_branch()
	{
		var target = defaultValue with { BranchName = infraBranchName };
		SetupInfraBranchDetails(incomingFromMain: 0, additionalCommits: 0);

		var branches = await target.Execute(fixture.Create());

		var actual = Assert.Single(branches);
		Assert.Equal(infraBranchName, actual.Name);
		Assert.Contains(baseBranchName, actual.DownstreamNames);
		Assert.Contains(parentFeatureBranchName, actual.DownstreamNames);
	}

	[Fact]
	public async Task Handle_when_specified_branch_is_missing()
	{
		var target = defaultValue;
		SetupBranchDoesNotExist(baseBranchName);
		SetupBranchExists(mainBranchName);
		SetupBranchExists(infraBranchName);
		SetupBranchExists(parentFeatureBranchName);

		var branches = await target.Execute(fixture.Create());

		var actual = Assert.Single(branches);
		Assert.Equal(baseBranchName, actual.Name);
		Assert.False(actual.Exists);
		Assert.All(actual.Upstreams, upstream => Assert.True(upstream.Exists));
		Assert.Contains(actual.Upstreams, upstream => upstream.Name == infraBranchName);
		Assert.Contains(actual.Upstreams, upstream => upstream.Name == parentFeatureBranchName);
	}

	[Fact]
	public async Task Handle_when_an_upstream_is_missing()
	{
		var target = defaultValue;
		SetupBranchExists(baseBranchName);
		SetupBranchExists(mainBranchName);
		SetupBranchDoesNotExist(infraBranchName);
		SetupBranchExists(parentFeatureBranchName);
		SetupGetCommitCount([infraBranchName], [baseBranchName], excludeMergeCommits: false).ReturnsAsync((int?)null);
		SetupGetCommitCount([parentFeatureBranchName], [baseBranchName], excludeMergeCommits: false).ReturnsAsync(0);
		SetupNoConflict(parentFeatureBranchName, baseBranchName);
		// infra branch does not exist, so will not be included in this commit count
		SetupGetCommitCount([baseBranchName], [mainBranchName, parentFeatureBranchName]).ReturnsAsync(0);

		var branches = await target.Execute(fixture.Create());

		var actual = Assert.Single(branches);
		Assert.Equal(baseBranchName, actual.Name);
		var infraBranchStatus = Assert.Single(actual.Upstreams, upstream => upstream.Name == infraBranchName);
		Assert.False(infraBranchStatus.Exists);
	}

	[Fact]
	public async Task Reveal_conflicts_with_an_upstream()
	{
		var target = defaultValue;
		SetupBranchExists(baseBranchName);
		SetupBranchExists(infraBranchName);
		SetupBranchExists(parentFeatureBranchName);
		SetupBranchExists(mainBranchName);
		SetupGetCommitCount([infraBranchName], [baseBranchName], excludeMergeCommits: false).ReturnsAsync(0);
		SetupConflict(infraBranchName, baseBranchName, ["readme.md"]);
		SetupGetCommitCount([parentFeatureBranchName], [baseBranchName], excludeMergeCommits: false).ReturnsAsync(0);
		SetupNoConflict(parentFeatureBranchName, baseBranchName);
		SetupGetCommitCount([baseBranchName], [mainBranchName, infraBranchName, parentFeatureBranchName]).ReturnsAsync(15);

		var branches = await target.Execute(fixture.Create());

		var actual = Assert.Single(branches);
		Assert.Equal(baseBranchName, actual.Name);
		var infraBranchStatus = Assert.Single(actual.Upstreams, upstream => upstream.Name == infraBranchName);
		Assert.True(infraBranchStatus.HasConflict);
		var parentFeatureBranchStatus = Assert.Single(actual.Upstreams, upstream => upstream.Name == parentFeatureBranchName);
		Assert.False(parentFeatureBranchStatus.HasConflict);
	}

	[Fact]
	public async Task Finds_upstream_branches_and_retrieves_details()
	{
		var target = defaultValue with { IncludeUpstream = true };
		SetupBaseBranchDetails(0, 0, additionalCommits: 15);
		SetupInfraBranchDetails(0, additionalCommits: 0);
		SetupParentBranchDetails(0, additionalCommits: 0);

		var branches = await target.Execute(fixture.Create());

		Assert.Contains(branches, (branch) => branch.Name == parentFeatureBranchName);
		Assert.Contains(branches, (branch) => branch.Name == infraBranchName);
		Assert.Contains(branches, (branch) => branch.Name == baseBranchName);
	}

	[Fact]
	public async Task Finds_upstream_branches_recursively_and_retrieves_details()
	{
		var target = defaultValue with { IncludeUpstream = true, Recurse = true };
		SetupBaseBranchDetails(0, 0, additionalCommits: 15);
		SetupInfraBranchDetails(0, additionalCommits: 0);
		SetupParentBranchDetails(0, additionalCommits: 0);

		SetupGetCommitCount([mainBranchName], []).ReturnsAsync(1500);

		var branches = await target.Execute(fixture.Create());

		Assert.Contains(branches, (branch) => branch.Name == parentFeatureBranchName);
		Assert.Contains(branches, (branch) => branch.Name == infraBranchName);
		Assert.Contains(branches, (branch) => branch.Name == baseBranchName);
		Assert.Contains(branches, (branch) => branch.Name == mainBranchName);
	}

	[Fact]
	public async Task Finds_downstream_branches_and_retrieves_details()
	{
		var target = defaultValue with { IncludeDownstream = true, BranchName = infraBranchName };
		SetupBaseBranchDetails(0, 0, additionalCommits: 0);
		SetupInfraBranchDetails(0, additionalCommits: 0);
		SetupParentBranchDetails(0, additionalCommits: 0);

		var branches = await target.Execute(fixture.Create());

		Assert.Contains(branches, (branch) => branch.Name == parentFeatureBranchName);
		Assert.Contains(branches, (branch) => branch.Name == infraBranchName);
		Assert.Contains(branches, (branch) => branch.Name == baseBranchName);
	}

	[Fact]
	public async Task Finds_downstream_branches_recursively_and_retrieves_details()
	{
		var target = defaultValue with { IncludeDownstream = true, Recurse = true, BranchName = mainBranchName };
		SetupBaseBranchDetails(0, 0, additionalCommits: 0);
		SetupInfraBranchDetails(0, additionalCommits: 0);
		SetupParentBranchDetails(0, additionalCommits: 0);

		SetupGetCommitCount([mainBranchName], []).ReturnsAsync(1500);

		var branches = await target.Execute(fixture.Create());

		Assert.Contains(branches, (branch) => branch.Name == parentFeatureBranchName);
		Assert.Contains(branches, (branch) => branch.Name == infraBranchName);
		Assert.Contains(branches, (branch) => branch.Name == baseBranchName);
		Assert.Contains(branches, (branch) => branch.Name == mainBranchName);
	}

	[Fact]
	public async Task Finds_upstream_and_downstream_branches_and_retrieves_details()
	{
		var target = defaultValue with { IncludeUpstream = true, IncludeDownstream = true, Recurse = true, BranchName = infraBranchName };
		SetupBaseBranchDetails(0, 0, additionalCommits: 0);
		SetupInfraBranchDetails(0, additionalCommits: 0);
		SetupParentBranchDetails(0, additionalCommits: 0);

		SetupGetCommitCount([mainBranchName], []).ReturnsAsync(1500);

		var branches = await target.Execute(fixture.Create());

		Assert.Contains(branches, (branch) => branch.Name == parentFeatureBranchName);
		Assert.Contains(branches, (branch) => branch.Name == infraBranchName);
		Assert.Contains(branches, (branch) => branch.Name == baseBranchName);
		Assert.Contains(branches, (branch) => branch.Name == mainBranchName);
	}

	private void SetupBaseBranchDetails(int incomingFromInfra, int incomingFromParent, int additionalCommits)
	{
		SetupBranchExists(baseBranchName);
		SetupBranchExists(infraBranchName);
		SetupBranchExists(parentFeatureBranchName);
		SetupBranchExists(mainBranchName);
		SetupGetCommitCount([infraBranchName], [baseBranchName], excludeMergeCommits: false).ReturnsAsync(incomingFromInfra);
		SetupNoConflict(infraBranchName, baseBranchName);
		SetupGetCommitCount([parentFeatureBranchName], [baseBranchName], excludeMergeCommits: false).ReturnsAsync(incomingFromParent);
		SetupNoConflict(parentFeatureBranchName, baseBranchName);
		SetupGetCommitCount([baseBranchName], [mainBranchName, infraBranchName, parentFeatureBranchName]).ReturnsAsync(additionalCommits);
	}

	private void SetupInfraBranchDetails(int incomingFromMain, int additionalCommits)
	{
		SetupBranchExists(infraBranchName);
		SetupBranchExists(mainBranchName);
		SetupGetCommitCount([mainBranchName], [infraBranchName], excludeMergeCommits: false).ReturnsAsync(incomingFromMain);
		SetupNoConflict(mainBranchName, infraBranchName);
		SetupGetCommitCount([infraBranchName], [mainBranchName]).ReturnsAsync(additionalCommits);
	}

	private void SetupParentBranchDetails(int incomingFromInfra, int additionalCommits)
	{
		SetupBranchExists(parentFeatureBranchName);
		SetupBranchExists(infraBranchName);
		SetupBranchExists(mainBranchName);
		SetupGetCommitCount([infraBranchName], [parentFeatureBranchName], excludeMergeCommits: false).ReturnsAsync(incomingFromInfra);
		SetupNoConflict(infraBranchName, parentFeatureBranchName);
		SetupGetCommitCount([parentFeatureBranchName], [mainBranchName, infraBranchName]).ReturnsAsync(additionalCommits);
	}

	private void SetupBranchExists(string branchName)
	{
		fixture.PowerShellCommandInvoker.Setup(i => i.RunCommand(new BranchExists(ToFullName(branchName)))).ReturnsAsync(true);
	}

	private void SetupBranchDoesNotExist(string branchName)
	{
		fixture.PowerShellCommandInvoker.Setup(i => i.RunCommand(new BranchExists(ToFullName(branchName)))).ReturnsAsync(false);
	}

	private ISetup<IPowerShellCommandInvoker, Task<int?>> SetupGetCommitCount(string[] included, string[] excluded, bool excludeMergeCommits = true)
	{
		return fixture.PowerShellCommandInvoker.Setup(i => i.RunCommand(It.Is<GetCommitCount>(cmd => MatchGetCommitCount(cmd, included, excluded, excludeMergeCommits))));
	}

	private bool MatchGetCommitCount(GetCommitCount cmd, string[] included, string[] excluded, bool excludeMergeCommits)
	{
		return cmd.ExcludeMergeCommits == excludeMergeCommits && cmd.Included.SequenceEqual(included.Select(ToFullName)) && cmd.Excluded.Order().SequenceEqual(excluded.Select(ToFullName).Order());
	}

	private void SetupNoConflict(string branch1, string branch2)
	{
		fixture.PowerShellCommandInvoker.Setup(i => i.RunCommand(new GetConflictingFiles(ToFullName(branch1), ToFullName(branch2))))
			.ReturnsAsync(new GetConflictingFilesResult(HasConflict: false, ResultTreeHash: "unused", [/*unused*/], [/*unused*/]));
	}

	private void SetupConflict(string branch1, string branch2, string[] conflictingFileNames)
	{
		fixture.PowerShellCommandInvoker.Setup(i => i.RunCommand(new GetConflictingFiles(ToFullName(branch1), ToFullName(branch2))))
			.ReturnsAsync(new GetConflictingFilesResult(HasConflict: true, ResultTreeHash: "unused", [/*unused*/], [/*unused*/]));
	}

}
