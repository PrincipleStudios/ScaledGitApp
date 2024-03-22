
using Moq;
using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

public class GitFetchShould
{
	private readonly PowerShellFixture fixture = new();

	[Fact]
	public async Task Issue_a_fetch_command()
	{
		var verifyGitFetch = SetupGitFetch(fixture.MockPowerShell);
		var target = new GitFetch();

		await target.RunCommand(fixture.Create());

		verifyGitFetch.Verify(Times.Once);
	}

	internal static VerifiableMock<IPowerShellCommandContext, Task<PowerShellInvocationResult>> SetupGitFetch(Mock<IPowerShellCommandContext> target)
	{
		return target.Verifiable(
			ps => ps.PowerShellInvoker.InvokeCliAsync("git", "fetch", "--porcelain", "--prune"),
			s => s.ReturnsAsync(PowerShellInvocationResultStubs.Empty)
		);
	}
}
