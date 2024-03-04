
using Moq;
using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

public class GitRemoteShould
{
	private readonly PowerShellFixture fixture = new();

	[Fact]
	public async Task Allow_no_remotes()
	{
		var verifyGitRemote = SetupGitRemote(fixture.MockPowerShell);
		var target = new GitRemote();

		var remotes = await target.RunCommand(fixture.Create());

		Assert.Empty(remotes.Remotes);
		verifyGitRemote.Verify(Times.Once);
	}

	[Fact]
	public async Task Allow_one_remote()
	{
		var expectedRemote = new GitRemoteEntry("origin", "https://example.com/.git");
		var verifyGitRemote = SetupGitRemote(fixture.MockPowerShell, [expectedRemote]);
		var target = new GitRemote();

		var remotes = await target.RunCommand(fixture.Create());

		var actualRemote = Assert.Single(remotes.Remotes);
		Assert.Equal(expectedRemote, actualRemote);
		verifyGitRemote.Verify(Times.Once);
	}

	[Fact]
	public async Task Allow_multiple_remotes()
	{
		var expectedRemote1 = new GitRemoteEntry("github", "https://example.com/1.git");
		var expectedRemote2 = new GitRemoteEntry("azure", "https://example.com/2.git");
		var verifyGitRemote = SetupGitRemote(fixture.MockPowerShell, [expectedRemote1, expectedRemote2]);
		var target = new GitRemote();

		var remotes = await target.RunCommand(fixture.Create());

		Assert.Collection(
			remotes.Remotes,
			(first) => Assert.Equal(expectedRemote1, first),
			(second) => Assert.Equal(expectedRemote2, second)
		);
		verifyGitRemote.Verify(Times.Once);
	}

	internal static VerifiableMock<IPowerShellCommandContext, Task<PowerShellInvocationResult>> SetupGitRemote(Mock<IPowerShellCommandContext> target, IEnumerable<GitRemoteEntry>? remotes = null)
	{
		return target.Verifiable(
			ps => ps.InvokeCliAsync("git", "remote", "-v"),
			s => s.ReturnsAsync(
				remotes == null
					? PowerShellInvocationResultStubs.Empty
					: PowerShellInvocationResultStubs.WithResults(
						remotes.SelectMany(r => new string[]
						{
							$"{r.Alias}\t{r.FetchUrl} (fetch)",
							$"{r.Alias}\t{r.FetchUrl} (push)"
						}).ToArray()
					)
			)
		);
	}
}
