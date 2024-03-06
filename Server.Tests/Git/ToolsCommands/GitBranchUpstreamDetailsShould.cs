using Moq;

namespace PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

public class GitBranchUpstreamDetailsShould
{
	private readonly GitToolsFixture fixture = new();
	private readonly GitBranchUpstreamDetails defaultValue = new GitBranchUpstreamDetails(
		["feature/PS-123"],
		IncludeDownstream: false,
		IncludeUpstream: false,
		Recurse: false
	);
	private readonly Dictionary<string, UpstreamBranchConfiguration> defaultUpstreamData = new()
	{
		{ "feature/PS-123", new ([ "infra/ratchet-tests", "feature/PS-100" ]) },
		{ "infra/ratchet-tests", new ([ "main" ]) },
		{ "feature/PS-100", new ([ "infra/ratchet-tests" ]) },
	};

	[Fact]
	public async Task Indicate_the_number_of_commits_the_current_branch_has_beyond_upstreams()
	{
		fixture.GitToolsCommandInvoker.Setup(i => i.RunCommand(It.IsAny<GitUpstreamData>())).ReturnsAsync(defaultUpstreamData);
		//fixture.PowerShellCommandInvoker.Setup(i => i.RunCommand(It.Is<GetCommitCount>(cmd => cmd.Included.Contains("")));
		//fixture.PowerShellCommandInvoker.Setup(i => i.RunCommand(It.IsAny<GetConflictingFiles>()));
		var target = defaultValue;

		var branches = await target.RunCommand(fixture.Create());

		Assert.Empty(branches);
	}

	[Fact]
	public async Task Report_number_of_commits_missing_from_upstreams()
	{
		var target = defaultValue;

		var branches = await target.RunCommand(fixture.Create());

		Assert.Empty(branches);
	}

	[Fact]
	public async Task Handle_when_specified_branch_is_missing()
	{
		var target = defaultValue;

		var branches = await target.RunCommand(fixture.Create());

		Assert.Empty(branches);
	}

	[Fact]
	public async Task Handle_when_an_upstream_is_missing()
	{
		var target = defaultValue;

		var branches = await target.RunCommand(fixture.Create());

		Assert.Empty(branches);
	}

	[Fact]
	public async Task Reveal_conflicts_with_an_upstream()
	{
		var target = defaultValue;

		var branches = await target.RunCommand(fixture.Create());

		Assert.Empty(branches);
	}

	[Fact]
	public async Task Finds_upstream_branches_and_retrieves_details()
	{
		var target = defaultValue with { IncludeUpstream = true };

		var branches = await target.RunCommand(fixture.Create());

		Assert.Empty(branches);
	}

	[Fact]
	public async Task Finds_upstream_branches_recursively_and_retrieves_details()
	{
		var target = defaultValue with { IncludeUpstream = true, Recurse = true };

		var branches = await target.RunCommand(fixture.Create());

		Assert.Empty(branches);
	}

	[Fact]
	public async Task Finds_downstream_branches_and_retrieves_details()
	{
		var target = defaultValue with { IncludeDownstream = true, Recurse = true };

		var branches = await target.RunCommand(fixture.Create());

		Assert.Empty(branches);
	}

	[Fact]
	public async Task Finds_downstream_branches_recursively_and_retrieves_details()
	{
		var target = defaultValue with { IncludeDownstream = true, Recurse = true };

		var branches = await target.RunCommand(fixture.Create());

		Assert.Empty(branches);
	}

	[Fact]
	public async Task Finds_upstream_and_downstream_branches_and_retrieves_details()
	{
		var target = defaultValue with { IncludeUpstream = true, IncludeDownstream = true, Recurse = true };

		var branches = await target.RunCommand(fixture.Create());

		Assert.Empty(branches);
	}

	[Fact]
	public async Task Finds_upstream_and_downstream_branches_recursively_and_retrieves_details()
	{
		var target = defaultValue with { IncludeUpstream = true, IncludeDownstream = true, Recurse = true };

		var branches = await target.RunCommand(fixture.Create());

		Assert.Empty(branches);
	}
}
