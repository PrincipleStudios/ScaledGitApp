
using Moq;
using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

public class GitFetchShould : IClassFixture<GitToolsPowerShellFixture>
{
	private readonly GitToolsPowerShellFixture fixture;

	public GitFetchShould(GitToolsPowerShellFixture fixture)
	{
		this.fixture = fixture;
	}

	[Fact]
	public async Task Issue_a_fetch_command()
	{
		var mockFinal = new Mock<IPowerShell>();
		fixture.MockPowerShellFactory.Setup(ps => ps.Create(null)).Returns(mockFinal.Object);

		var verifyGitFetch = SetupGitFetch(mockFinal);

		// By mocking the factory directly, we test the typical DI constructor with working directory setup
		using var target = fixture.CreateTarget();

		await target.GitFetch();

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
