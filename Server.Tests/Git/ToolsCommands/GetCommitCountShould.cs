using Moq;
using PrincipleStudios.ScaledGitApp.ShellUtilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

public class GetCommitCountShould
{
	private readonly PowerShellFixture fixture = new();

	[Fact]
	public async Task Get_commit_count_of_a_branch()
	{
		var included = new string[] { "main" };
		var verifyGitClone = SetupGetCommitCount(fixture.MockPowerShell, included, [], 15, excludeMergeCommits: true);
		var target = new GetCommitCount(included, []);

		var actual = await target.Execute(fixture.Create());

		Assert.Equal(15, actual);
		verifyGitClone.Verify(Times.Once);
	}

	[Fact]
	public async Task Get_commit_count_of_a_branch_including_merge_commits()
	{
		var included = new string[] { "main" };
		var verifyGitClone = SetupGetCommitCount(fixture.MockPowerShell, included, [], 15, excludeMergeCommits: false);
		var target = new GetCommitCount(included, [], ExcludeMergeCommits: false);

		var actual = await target.Execute(fixture.Create());

		Assert.Equal(15, actual);
		verifyGitClone.Verify(Times.Once);
	}

	[Fact]
	public async Task Get_commit_count_of_multiple_branches()
	{
		var included = new string[] { "main", "feature/PS-123" };
		var verifyGitClone = SetupGetCommitCount(fixture.MockPowerShell, included, [], 15, excludeMergeCommits: true);
		var target = new GetCommitCount(included, []);

		var actual = await target.Execute(fixture.Create());

		Assert.Equal(15, actual);
		verifyGitClone.Verify(Times.Once);
	}

	[Fact]
	public async Task Get_commit_count_of_multiple_branches_including_merge_commits()
	{
		var included = new string[] { "main", "feature/PS-123" };
		var verifyGitClone = SetupGetCommitCount(fixture.MockPowerShell, included, [], 15, excludeMergeCommits: false);
		var target = new GetCommitCount(included, [], ExcludeMergeCommits: false);

		var actual = await target.Execute(fixture.Create());

		Assert.Equal(15, actual);
		verifyGitClone.Verify(Times.Once);
	}

	[Fact]
	public async Task Get_commit_count_excluding_branches()
	{
		var included = new string[] { "feature/PS-123" };
		var excluded = new string[] { "main" };
		var verifyGitClone = SetupGetCommitCount(fixture.MockPowerShell, included, excluded, 1, excludeMergeCommits: true);
		var target = new GetCommitCount(included, excluded);

		var actual = await target.Execute(fixture.Create());

		Assert.Equal(1, actual);
		verifyGitClone.Verify(Times.Once);
	}

	[Fact]
	public async Task Get_commit_count_excluding_branches_with_merge_commits()
	{
		var included = new string[] { "feature/PS-123" };
		var excluded = new string[] { "main" };
		var verifyGitClone = SetupGetCommitCount(fixture.MockPowerShell, included, excluded, 3, excludeMergeCommits: false);
		var target = new GetCommitCount(included, excluded, ExcludeMergeCommits: false);

		var actual = await target.Execute(fixture.Create());

		Assert.Equal(3, actual);
		verifyGitClone.Verify(Times.Once);
	}

	internal static VerifiableMock<IPowerShellCommandContext, Task<PowerShellInvocationResult>> SetupGetCommitCount(Mock<IPowerShellCommandContext> target, string[] included, string[] excluded, int count, bool excludeMergeCommits)
	{
		return target.Verifiable(
			ps => ps.PowerShellInvoker.InvokeCliAsync("git", It.Is<string[]>(args => VerifyCliArgs(args, included, excluded, excludeMergeCommits))),
			s => s.ReturnsAsync(PowerShellInvocationResultStubs.WithResults(count.ToString()))
		);
	}

	static readonly string[] switchesWithoutMergeCommits = ["--count", "--no-merges"];
	static readonly string[] switchesWithMergeCommits = ["--count"];
	static bool VerifyCliArgs(string[] args, string[] included, string[] excluded, bool excludeMergeCommits)
	{
		if (args[0] != "rev-list") return false;
		var argset = new HashSet<string>(args.Skip(1));
		var switches = excludeMergeCommits ? switchesWithoutMergeCommits : switchesWithMergeCommits;
		foreach (var s in switches)
			if (!argset.Remove(s))
				return false;
		foreach (var s in included)
			if (!argset.Remove(s))
				return false;
		foreach (var s in excluded)
			if (!argset.Remove('^' + s))
				return false;

		if (argset.Count > 0) return false;
		return true;
	}
}
