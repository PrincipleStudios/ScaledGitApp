using Moq;
using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

public class GetConflictingFilesShould
{
	private readonly PowerShellFixture fixture = new PowerShellFixture();

	[Fact]
	public async Task Return_new_hash_if_no_conflicts()
	{
		var branch1 = "feature/PS-123";
		var branch2 = "infra/auto-format";
		var expectedTree = "0123456789012345678901234567890123456789";
		var verifyGitClone = SetupConflictingFiles(fixture.MockPowerShell, [branch1, branch2], expectedTree, []);
		var target = new GetConflictingFiles(branch1, branch2);

		var actual = await target.RunCommand(fixture.Create());

		Assert.False(actual.HasConflict);
		Assert.Equal(expectedTree, actual.ResultTreeHash);
		Assert.Empty(actual.ConflictingFileNames);
		verifyGitClone.Verify(Times.Once);
	}

	[Fact]
	public async Task Return_hash_with_list_of_files_if_conflicts_exist()
	{
		var branch1 = "feature/PS-123";
		var branch2 = "infra/auto-format";
		var expectedTree = "0123456789012345678901234567890123456789";
		var conflictingFiles = new string[]
		{
			"README.md",
			"some/long/file.ts"
		};
		var verifyGitClone = SetupConflictingFiles(fixture.MockPowerShell, [branch1, branch2], expectedTree, conflictingFiles);
		var target = new GetConflictingFiles(branch1, branch2);

		var actual = await target.RunCommand(fixture.Create());

		Assert.True(actual.HasConflict);
		Assert.Equal(expectedTree, actual.ResultTreeHash);
		Assert.Equivalent(conflictingFiles, actual.ConflictingFileNames);
		verifyGitClone.Verify(Times.Once);
	}

	internal static VerifiableMock<IPowerShellCommandContext, Task<PowerShellInvocationResult>> SetupConflictingFiles(Mock<IPowerShellCommandContext> target, string[] branches, string resultTreeHash, string[] conflictingFiles)
	{
		return target.Verifiable(
			ps => ps.PowerShellInvoker.InvokeCliAsync("git", It.Is<string[]>(args => VerifyCliArgs(args, branches))),
			s => s.ReturnsAsync(PowerShellInvocationResultStubs.WithResults(conflictingFiles.Prepend(resultTreeHash).ToArray()) with
			{
				HadErrors = conflictingFiles is not []
			})
		);
	}

	static readonly string[] defaultSwitches = ["--name-only", "--no-messages"];
	static bool VerifyCliArgs(string[] args, string[] branches)
	{
		if (args[0] != "merge-tree") return false;
		var argset = new HashSet<string>(args.Skip(1));
		foreach (var s in defaultSwitches)
			if (!argset.Remove(s))
				return false;
		foreach (var s in branches)
			if (!argset.Remove(s))
				return false;

		if (argset.Count > 0) return false;
		return true;
	}
}
