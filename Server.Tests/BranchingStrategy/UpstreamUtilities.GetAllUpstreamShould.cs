using PrincipleStudios.ScaledGitApp.Git;
using PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

namespace PrincipleStudios.ScaledGitApp.BranchingStrategy;

public class UpstreamUtilities_GetAllUpstream_should
{
	private readonly GitToolsFixture fixture = new();

	private async Task VerifyGetAllUpstreams(string targetBranch, string[] expectedUpstreamBranches, Dictionary<string, string[]> branchConfigurations)
	{
		GitUpstreamDataShould.SetupGitUpstreamDataBranches(fixture.MockPowerShell, branchConfigurations);
		var target = new GitUpstreamData();
		var branches = await target.Execute(fixture.Create());

		var result = branches.GetAllUpstream(targetBranch);

		Assert.Collection(
			result.Order(),
			expectedUpstreamBranches
				.Order()
				.Select(expected => (Action<string>)((actual) => Assert.Equal(expected, actual)))
				.ToArray()
		);
	}


	private static readonly Dictionary<string, string[]> branchConfigurations = new()
		{
			{ "feature/PS-123", ["main"] },
			{ "feature/PS-124", ["feature/PS-123", "infra/upgrade"] },
			{ "infra/upgrade", ["main"] },
		};
	[Fact]
	public Task Report_no_branches_if_not_configured() =>
		VerifyGetAllUpstreams("main", [], branchConfigurations);
	[Fact]
	public Task Report_direct_upstreams() =>
		VerifyGetAllUpstreams("feature/PS-123", ["main"], branchConfigurations);
	[Fact]
	public Task Report_recursive_upstreams_without_repeating() =>
		VerifyGetAllUpstreams("feature/PS-124", ["main", "infra/upgrade", "feature/PS-123"], branchConfigurations);


	// A recursive configuration should not happen, but we need to test to make sure it doesn't break
	// It is acceptable in this case to state that feature/a is upstream of itself
	private static readonly Dictionary<string, string[]> recursiveConfiguration = new()
		{
			{ "feature/a", ["feature/c"] },
			{ "feature/b", ["feature/a"] },
			{ "feature/c", ["feature/b"] },
		};
	[Theory]
	[InlineData("feature/a")]
	[InlineData("feature/b")]
	[InlineData("feature/c")]
	public Task Handle_recursive_upstreams(string branch) =>
		VerifyGetAllUpstreams(branch, ["feature/a", "feature/b", "feature/c"], recursiveConfiguration);
}
