using Moq;
using PrincipleStudios.ScaledGitApp.ShellUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

public class GetCommitCountShould
{
	private readonly PowerShellFixture fixture = new PowerShellFixture();

	[Fact]
	public async Task Get_commit_count_of_a_branch()
	{
		var included = new string[] { "main" };
		var verifyGitClone = SetupGetCommitCount(fixture.MockPowerShell, included, [], 15);
		var target = new GetCommitCount(included, []);

		await target.RunCommand(fixture.Create());

		verifyGitClone.Verify(Times.Once);
	}

	[Fact]
	public async Task Get_commit_count_of_multiple_branches()
	{
		var included = new string[] { "main", "feature/PS-123" };
		var verifyGitClone = SetupGetCommitCount(fixture.MockPowerShell, included, [], 15);
		var target = new GetCommitCount(included, []);

		await target.RunCommand(fixture.Create());

		verifyGitClone.Verify(Times.Once);
	}

	[Fact]
	public async Task Get_commit_count_excluding_branches()
	{
		var included = new string[] { "feature/PS-123" };
		var excluded = new string[] { "main" };
		var verifyGitClone = SetupGetCommitCount(fixture.MockPowerShell, included, excluded, 1);
		var target = new GetCommitCount(included, excluded);

		await target.RunCommand(fixture.Create());

		verifyGitClone.Verify(Times.Once);
	}

	internal static VerifiableMock<IPowerShellCommandContext, Task<PowerShellInvocationResult>> SetupGetCommitCount(Mock<IPowerShellCommandContext> target, string[] included, string[] excluded, int count)
	{
		return target.Verifiable(
			ps => ps.PowerShellInvoker.InvokeCliAsync("git", It.Is<string[]>(args => VerifyCliArgs(args, included, excluded))),
			s => s.ReturnsAsync(PowerShellInvocationResultStubs.WithResults(count.ToString()))
		);
	}

	static readonly string[] defaultSwitches = ["--count", "--no-merges"];
	static bool VerifyCliArgs(string[] args, string[] included, string[] excluded)
	{
		if (args[0] != "rev-list") return false;
		var argset = new HashSet<string>(args.Skip(1));
		foreach (var s in defaultSwitches)
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
