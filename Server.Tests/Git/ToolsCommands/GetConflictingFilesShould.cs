using Microsoft.Extensions.Logging;
using Moq;
using PrincipleStudios.ScaledGitApp.ShellUtilities;
using System.Management.Automation;
using System.Text;

namespace PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

public class GetConflictingFilesShould
{
	private readonly PowerShellFixture fixture = new();


	/// <summary>
	/// This test runs actual git commands based on the git repository at the time this was checked into. 
	/// </summary>
	[Fact]
	[Trait("git-history", "required")]
	public async Task Work_with_actual_git_commands()
	{
		// This shows an actual merge that was fixed in this project's history:
		// git merge-tree -z f99c129e247838b61f7483aed9f8400609e1591d 0ebd73d8aaa192a93fb3fc3fd2ef8e6bcba2231e
		var branch1 = "f99c129e247838b61f7483aed9f8400609e1591d";
		var branch2 = "0ebd73d8aaa192a93fb3fc3fd2ef8e6bcba2231e";
		var expectedTree = "a04d52d2e76a849755fef2c65dfe9fadbf8f097a";
		using var pwsh = PowerShell.Create();
		var context = await ConstructActualContext(pwsh);
		var target = new GetConflictingFiles(branch1, branch2);

		var actual = await target.Execute(context);

		Assert.True(actual.HasConflict);
		Assert.Equal(expectedTree, actual.ResultTreeHash);
		Assert.Collection(actual.ConflictingFiles,
			conflictingFile =>
			{
				Assert.Equal("ui/src/pages/branch-details/index.tsx", conflictingFile.FilePath);
				Assert.Equal("100644", conflictingFile.Left?.Mode);
				Assert.Equal("100644", conflictingFile.Right?.Mode);
				Assert.Equal("100644", conflictingFile.MergeBase?.Mode);
				Assert.Equal("52c2aa8fd873b3e75d7752b33f27e92b6ceaa323", conflictingFile.Left?.Hash);
				Assert.Equal("01a6b2584577f1521ad1ec6dc816969add67d997", conflictingFile.Right?.Hash);
				Assert.Equal("709107784099844e065eab8aa2d822fca39bf4d9", conflictingFile.MergeBase?.Hash);
			});
		Assert.Collection(actual.ConflictMessages,
			(message) =>
			{
				Assert.Collection(message.FilePaths, (path) => Assert.Equal("ui/src/pages/branch-details/index.tsx", path));
				Assert.Equal("Auto-merging", message.ConflictType);
				Assert.Equal("Auto-merging ui/src/pages/branch-details/index.tsx", message.Message);
			},
			(message) =>
			{
				Assert.Collection(message.FilePaths, (path) => Assert.Equal("ui/src/pages/branch-details/index.tsx", path));
				Assert.Equal("CONFLICT (contents)", message.ConflictType);
				Assert.Equal("CONFLICT (content): Merge conflict in ui/src/pages/branch-details/index.tsx", message.Message);
			});
	}

	private static async Task<PowerShellCommandContext> ConstructActualContext(PowerShell pwsh)
	{
		PowerShellWrapperImplementation powerShellInvoker = new(pwsh);
		PowerShellCommandContext context = new(powerShellInvoker, Mock.Of<ILogger>(), Mock.Of<Commands.ICommandCache>());
		pwsh.SetCurrentWorkingDirectory(await new ResolveTopLevelDirectory().Execute(context));
		return context;
	}

	[Fact]
	public async Task Return_new_hash_if_no_conflicts()
	{
		var branch1 = "feature/PS-123";
		var branch2 = "infra/auto-format";
		var expectedTree = "0123456789012345678901234567890123456789";
		var verifyGitClone = SetupConflictingFiles(fixture.MockPowerShell, [branch1, branch2], expectedTree, []);
		var target = new GetConflictingFiles(branch1, branch2);

		var actual = await target.Execute(fixture.Create());

		Assert.False(actual.HasConflict);
		Assert.Equal(expectedTree, actual.ResultTreeHash);
		Assert.Empty(actual.ConflictingFiles);
		Assert.Empty(actual.ConflictMessages);
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

		var actual = await target.Execute(fixture.Create());

		Assert.True(actual.HasConflict);
		Assert.Equal(expectedTree, actual.ResultTreeHash);
		Assert.Equivalent(conflictingFiles, actual.ConflictingFiles.Select(p => p.FilePath));
		verifyGitClone.Verify(Times.Once);
	}

	/// <summary>
	/// This occurs if one of the specified branches does not exist
	/// </summary>
	[Fact]
	public async Task Handle_failed_commands_in_an_expected_way()
	{
		var branch1 = "feature/PS-123";
		var branch2 = "infra/auto-format";
		var verifyGitClone = SetupFailedMerge(fixture.MockPowerShell, [branch1, branch2]);
		var target = new GetConflictingFiles(branch1, branch2);

		await Assert.ThrowsAsync<GitException>(() => target.Execute(fixture.Create()));

		verifyGitClone.Verify(Times.Once);
	}

	internal static VerifiableMock<IPowerShellCommandContext, Task<PowerShellInvocationResult>> SetupConflictingFiles(Mock<IPowerShellCommandContext> target, string[] branches, string resultTreeHash, string[] conflictingFiles)
	{
		var resultText = new StringBuilder();
		resultText.Append(resultTreeHash).Append('\0');
		// mode ' ' hash ' ' stage '\t' path
		foreach (var (file, index) in conflictingFiles.Select((file, index) => (file, index)))
		{
			resultText.Append($"100466 {(index * 3 + 0).ToString("40X")} 1\t{file}").Append('\0');
			resultText.Append($"100466 {(index * 3 + 1).ToString("40X")} 2\t{file}").Append('\0');
			resultText.Append($"100466 {(index * 3 + 2).ToString("40X")} 3\t{file}").Append('\0');
		}
		resultText.Append('\0');
		// conflict messages
		foreach (var file in conflictingFiles)
		{
			resultText
				.Append(1).Append('\0')
				.Append(file).Append('\0')
				.Append("CONFLICT (contents)").Append('\0')
				.Append($"CONFLICT (content): Merge conflict in {file}\n").Append('\0');
		}


		return target.Verifiable(
			ps => ps.PowerShellInvoker.InvokeCliAsync("git", It.Is<string[]>(args => VerifyCliArgs(args, branches))),
			s => s.ReturnsAsync(PowerShellInvocationResultStubs.WithResults(resultText.ToString().Split('\n')) with
			{
				LastExitCode = conflictingFiles is [] ? 0 : 1
			})
		);
	}

	internal static VerifiableMock<IPowerShellCommandContext, Task<PowerShellInvocationResult>> SetupFailedMerge(Mock<IPowerShellCommandContext> target, string[] branches)
	{
		return target.Verifiable(
			ps => ps.PowerShellInvoker.InvokeCliAsync("git", It.Is<string[]>(args => VerifyCliArgs(args, branches))),
			s => s.ReturnsAsync(PowerShellInvocationResultStubs.Empty with
			{
				LastExitCode = 2,
			})
		);
	}

	static readonly string[] defaultSwitches = ["-z"];
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
