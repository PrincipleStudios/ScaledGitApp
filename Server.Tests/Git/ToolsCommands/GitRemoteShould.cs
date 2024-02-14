
using Moq;
using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

public class GitRemoteShould : IClassFixture<GitToolsPowerShellFixture>
{
	private readonly GitToolsPowerShellFixture fixture;

	public GitRemoteShould(GitToolsPowerShellFixture fixture)
	{
		this.fixture = fixture;
	}

	[Fact]
	public async Task Allow_no_remotes()
	{
		var mockFinal = new Mock<IPowerShell>();
		fixture.MockPowerShellFactory.Setup(ps => ps.Create(null)).Returns(mockFinal.Object);

		var verifyGitRemote = SetupGitRemote(mockFinal);

		// By mocking the factory directly, we test the typical DI constructor with working directory setup
		using var target = fixture.CreateTarget();

		var remotes = await target.GitRemote();

		Assert.Empty(remotes.Remotes);
		verifyGitRemote.Verify(Times.Once);
	}

	[Fact]
	public async Task Allow_one_remote()
	{
		var expectedRemote = new GitRemote("origin", "https://example.com/.git");
		var mockFinal = new Mock<IPowerShell>();
		fixture.MockPowerShellFactory.Setup(ps => ps.Create(null)).Returns(mockFinal.Object);

		var verifyGitRemote = SetupGitRemote(mockFinal, [expectedRemote]);

		// By mocking the factory directly, we test the typical DI constructor with working directory setup
		using var target = fixture.CreateTarget();

		var remotes = await target.GitRemote();

		var actualRemote = Assert.Single(remotes.Remotes);
		Assert.Equal(expectedRemote, actualRemote);
		verifyGitRemote.Verify(Times.Once);
	}

	[Fact]
	public async Task Allow_multiple_remotes()
	{
		var expectedRemote1 = new GitRemote("github", "https://example.com/1.git");
		var expectedRemote2 = new GitRemote("azure", "https://example.com/2.git");
		var mockFinal = new Mock<IPowerShell>();
		fixture.MockPowerShellFactory.Setup(ps => ps.Create(null)).Returns(mockFinal.Object);

		var verifyGitRemote = SetupGitRemote(mockFinal, [expectedRemote1, expectedRemote2]);

		// By mocking the factory directly, we test the typical DI constructor with working directory setup
		using var target = fixture.CreateTarget();

		var remotes = await target.GitRemote();

		Assert.Collection(
			remotes.Remotes,
			(first) => Assert.Equal(expectedRemote1, first),
			(second) => Assert.Equal(expectedRemote2, second)
		);
		verifyGitRemote.Verify(Times.Once);
	}

	internal static VerifiableMock<IPowerShell, Task<PowerShellInvocationResult>> SetupGitRemote(Mock<IPowerShell> target, IEnumerable<GitRemote>? remotes = null)
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
