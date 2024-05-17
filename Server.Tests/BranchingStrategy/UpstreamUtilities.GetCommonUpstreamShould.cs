using PrincipleStudios.ScaledGitApp.Git;
using PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

namespace PrincipleStudios.ScaledGitApp.BranchingStrategy;

public class UpstreamUtilities_GetCommonUpstream_should
{
	private readonly GitToolsFixture fixture = new();

	private async Task VerifyGetCommonUpstreams(string[] targetBranches, string[] expectedUpstreamBranches, Dictionary<string, string[]> branchConfigurations)
	{
		GitUpstreamDataShould.SetupGitUpstreamDataBranches(fixture.MockPowerShell, branchConfigurations);
		var target = new GitUpstreamData();
		var branches = await target.Execute(fixture.Create());

		var result = branches.GetCommonUpstream(targetBranches);

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
			{ "feature/PS-125", ["infra/upgrade"] },
			{ "infra/upgrade", ["main"] },
		};
	[Fact]
	public Task Report_itself_if_not_configured() =>
		VerifyGetCommonUpstreams(["main"], ["main"], branchConfigurations);
	[Fact]
	public Task Report_direct_upstreams() =>
		VerifyGetCommonUpstreams(["feature/PS-123"], ["feature/PS-123", "main"], branchConfigurations);
	[Fact]
	public Task Report_recursive_upstreams_without_repeating() =>
		VerifyGetCommonUpstreams(["feature/PS-124"], ["main", "infra/upgrade", "feature/PS-123", "feature/PS-124"], branchConfigurations);
	[Fact]
	public Task Report_common_branch_in_input() =>
		VerifyGetCommonUpstreams(["feature/PS-124", "main"], ["main"], branchConfigurations);
	[Fact]
	public Task Report_common_branches_upstream() =>
		VerifyGetCommonUpstreams(["feature/PS-123", "infra/upgrade"], ["main"], branchConfigurations);
	[Fact]
	public Task Report_unnamed_common_upstream() =>
		VerifyGetCommonUpstreams(["feature/PS-124", "feature/PS-125"], ["infra/upgrade", "main"], branchConfigurations);

}
