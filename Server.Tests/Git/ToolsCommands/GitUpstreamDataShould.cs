using Moq;
using PrincipleStudios.ScaledGitApp.ShellUtilities;
using System.Management.Automation;

namespace PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

public class GitUpstreamDataShould
{
	private readonly GitToolsFixture fixture = new();

	[Fact]
	public async Task Handles_missing_the_upstream_branch()
	{
		var verifyGitRemote = SetupGitUpstreamDataFailure(fixture.MockPowerShell);
		var target = new GitUpstreamData();

		var branches = await target.Execute(fixture.Create());

		Assert.Empty(branches);
		verifyGitRemote.Verify(Times.Once);
	}

	[Fact]
	public async Task Handles_no_upstream_branch_configuration()
	{
		Dictionary<string, string[]> branchConfigurations = new();
		var verifyGitRemote = SetupGitUpstreamDataBranches(fixture.MockPowerShell, branchConfigurations);
		var target = new GitUpstreamData();

		var branches = await target.Execute(fixture.Create());

		AssertAllBranchConfigurations(branchConfigurations, branches);
		verifyGitRemote.Verify(Times.Once);
	}

	[Fact]
	public async Task Handles_a_single_upstream_configuration()
	{
		Dictionary<string, string[]> branchConfigurations = new()
		{
			{ "feature/PS-123", ["main"] },
		};
		var verifyGitRemote = SetupGitUpstreamDataBranches(fixture.MockPowerShell, branchConfigurations);
		var target = new GitUpstreamData();

		var branches = await target.Execute(fixture.Create());

		AssertAllBranchConfigurations(branchConfigurations, branches);
		verifyGitRemote.Verify(Times.Once);
	}

	[Fact]
	public async Task Handles_multiple_upstream_configurations()
	{
		Dictionary<string, string[]> branchConfigurations = new()
		{
			{ "feature/PS-123", ["main"] },
			{ "feature/PS-124", ["feature/PS-123", "infra/upgrade"] },
			{ "infra/upgrade", ["main"] },
		};
		var verifyGitRemote = SetupGitUpstreamDataBranches(fixture.MockPowerShell, branchConfigurations);
		var target = new GitUpstreamData();

		var actualConfigurations = await target.Execute(fixture.Create());

		AssertAllBranchConfigurations(branchConfigurations, actualConfigurations);
		verifyGitRemote.Verify(Times.Once);
	}

	private static void AssertAllBranchConfigurations(IReadOnlyDictionary<string, string[]> expectedResult, IReadOnlyDictionary<string, UpstreamBranchConfiguration> actualResult)
	{
		Assert.All(actualResult.Keys, (branchName) => { Assert.Contains(branchName, expectedResult); });
		foreach (var (expectedBranchName, expectedConfiguration) in expectedResult)
		{
			var actualConfiguration = Assert.Contains(expectedBranchName, actualResult);
			foreach (var branch in expectedConfiguration)
				Assert.Contains(branch, actualConfiguration.UpstreamBranchNames);
		}
	}

	internal static VerifiableMock<IPowerShell, Task<PowerShellInvocationResult>> SetupGitUpstreamDataFailure(Mock<IPowerShell> target)
	{
		return target.Verifiable(
			ps => ps.InvokeCliAsync("git", "ls-tree", "-r", "refs/remotes/origin/_upstream", "--format=%(objectname) %(path)"),
			s => s.ReturnsAsync(PowerShellInvocationResultStubs.WithCliErrors(
				128,
				"fatal: Not a valid object name refs/remotes/origin/_upstream"
			))
		);
	}

	internal static VerifiableMock<IPowerShell, Task<PowerShellInvocationResult>> SetupGitUpstreamDataBranches(Mock<IPowerShell> target, Dictionary<string, string[]> expectedResult)
	{
		var entries = expectedResult.Select(kvp => (Path: kvp.Key, Hash: string.Join('\n', kvp.Value).GetHashCode().ToString("X40"), Branches: kvp.Value));

		target.Setup(ps => ps.InvokeCliAsync("git", It.IsAny<PSDataCollection<string>>(), It.Is<IEnumerable<string>>(s => s.First() == "cat-file" && s.ElementAt(1) == "--batch=\t%(objectname)")))
			.ReturnsAsync(PowerShellInvocationResultStubs.WithResults(
				(from e in entries
				 from line in e.Branches.Prepend($"\t{e.Hash}").Append("")
				 select line).Distinct().Prepend("").ToArray()
			));

		return target.Verifiable(
			ps => ps.InvokeCliAsync("git", "ls-tree", "-r", "refs/remotes/origin/_upstream", "--format=%(objectname) %(path)"),
			s => s.ReturnsAsync(PowerShellInvocationResultStubs.WithResults(
				entries.Select(e => $"{e.Hash} {e.Path}").ToArray()
			))
		);
	}
}
