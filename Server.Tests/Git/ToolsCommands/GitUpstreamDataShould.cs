using Moq;
using PrincipleStudios.ScaledGitApp.ShellUtilities;
using System.Management.Automation;

namespace PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

public class GitUpstreamDataShould
{
	private readonly GitToolsPowerShellFixture fixture;

	public GitUpstreamDataShould()
	{
		this.fixture = new GitToolsPowerShellFixture();
	}

	[Fact]
	public async Task Handles_missing_the_upstream_branch()
	{
		var verifyGitRemote = SetupGitUpstreamDataFailure(fixture.MockPowerShell);
		var target = new GitUpstreamData();

		var branches = await target.RunCommand(fixture.Create());

		Assert.Empty(branches);
		verifyGitRemote.Verify(Times.Once);
	}

	[Fact]
	public async Task Handles_a_single_upstream_configuration()
	{
		var verifyGitRemote = SetupGitUpstreamDataBranches(fixture.MockPowerShell, new()
		{
			{ "feature/PS-123", ["main"] },
		});
		var target = new GitUpstreamData();

		var branches = await target.RunCommand(fixture.Create());

		var (branchName, config) = Assert.Single(branches);
		Assert.Equal("feature/PS-123", branchName);
		Assert.Collection(config.UpstreamBranchNames,
			actual => Assert.Equal("main", actual)
		);
		verifyGitRemote.Verify(Times.Once);
	}

	[Fact]
	public async Task Handles_multiple_upstream_configurations()
	{
		var verifyGitRemote = SetupGitUpstreamDataBranches(fixture.MockPowerShell, new()
		{
			{ "feature/PS-123", ["main"] },
			{ "feature/PS-124", ["feature/PS-123", "infra/upgrade"] },
			{ "infra/upgrade", ["main"] },
		});
		var target = new GitUpstreamData();

		var branches = await target.RunCommand(fixture.Create());

		var mainOnlyUpstream = Assert.Contains("feature/PS-123", branches);
		Assert.Collection(mainOnlyUpstream.UpstreamBranchNames,
			actual => Assert.Equal("main", actual)
		);
		var mainOnlyUpstream2 = Assert.Contains("infra/upgrade", branches);
		Assert.Collection(mainOnlyUpstream2.UpstreamBranchNames,
			actual => Assert.Equal("main", actual)
		);
		var multipleUpstream = Assert.Contains("feature/PS-124", branches);
		Assert.Contains("feature/PS-123", multipleUpstream.UpstreamBranchNames);
		Assert.Contains("infra/upgrade", multipleUpstream.UpstreamBranchNames);
		verifyGitRemote.Verify(Times.Once);
	}

	internal static VerifiableMock<IPowerShell, Task<PowerShellInvocationResult>> SetupGitUpstreamDataFailure(Mock<IPowerShell> target)
	{
		return target.Verifiable(
			ps => ps.InvokeCliAsync("git", "ls-tree", "-r", "origin/_upstream", "--format=%(objectname) %(path)"),
			s => s.ReturnsAsync(PowerShellInvocationResultStubs.WithCliErrors(
				"fatal: Not a valid object name origin/_upstream"
			))
		);
	}

	internal static VerifiableMock<IPowerShell, Task<PowerShellInvocationResult>> SetupGitUpstreamDataBranches(Mock<IPowerShell> target, Dictionary<string, string[]> branchConfigurations)
	{
		var entries = branchConfigurations.Select(kvp => (Path: kvp.Key, Hash: string.Join('\n', kvp.Value).GetHashCode().ToString("X40"), Branches: kvp.Value));

		target.Setup(ps => ps.InvokeCliAsync("git", It.IsAny<PSDataCollection<string>>(), It.Is<IEnumerable<string>>(s => s.First() == "cat-file" && s.ElementAt(1) == "--batch=\t%(objectname)")))
			.ReturnsAsync(PowerShellInvocationResultStubs.WithResults(
				(from e in entries
				 from line in e.Branches.Prepend($"\t{e.Hash}").Append("")
				 select line).Distinct().Prepend("").ToArray()
			));

		return target.Verifiable(
			ps => ps.InvokeCliAsync("git", "ls-tree", "-r", "origin/_upstream", "--format=%(objectname) %(path)"),
			s => s.ReturnsAsync(PowerShellInvocationResultStubs.WithResults(
				entries.Select(e => $"{e.Hash} {e.Path}").ToArray()
			))
		);
	}
}
