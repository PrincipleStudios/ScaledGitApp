
using Moq;
using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

public class GitFetchShould
{
	private readonly GitToolsPowerShellFixture fixture = new GitToolsPowerShellFixture();

	[Fact]
	public async Task Issue_a_fetch_command()
	{
		var verifyGitFetch = SetupGitFetch(fixture.MockPowerShell);
		var target = new GitFetch();

		await target.RunCommand(fixture.Create());

		verifyGitFetch.Verify(Times.Once);
	}

	internal static VerifiableMock<IPowerShell, Task<PowerShellInvocationResult>> SetupGitFetch(Mock<IPowerShell> target)
	{
		return target.Verifiable(
			ps => ps.InvokeCliAsync("git", "fetch", "--porcelain"),
			s => s.ReturnsAsync(PowerShellInvocationResultStubs.Empty)
		);
	}
}
