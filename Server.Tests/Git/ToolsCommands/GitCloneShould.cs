using Moq;
using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

public class GitCloneShould : IClassFixture<GitToolsPowerShellFixture>
{
	private readonly GitToolsPowerShellFixture fixture;

	public GitCloneShould(GitToolsPowerShellFixture fixture)
	{
		this.fixture = fixture;
	}

	[Fact]
	public async Task Issue_a_clone()
	{
		var expectedRepository = "https://example.com/.git";
		var mockFinal = new Mock<IPowerShell>();
		fixture.MockPowerShellFactory.Setup(ps => ps.Create(null)).Returns(mockFinal.Object);

		var verifyGitClone = SetupGitClone(mockFinal, expectedRepository);

		// By mocking the factory directly, we test the typical DI constructor with working directory setup
		using var target = fixture.CreateTarget();

		await target.GitClone(expectedRepository);

		verifyGitClone.Verify(Times.Once);
	}

	internal static VerifiableMock<IPowerShell, Task<PowerShellInvocationResult>> SetupGitClone(Mock<IPowerShell> target, string expectedRepository)
	{
		// This is very permissive right now; clone should be bare, and then we don't need the other setups
		target.Setup(ps => ps.InvokeCliAsync("git", It.IsAny<string[]>())).ReturnsAsync(PowerShellInvocationResultStubs.Empty);
		target.Setup(ps => ps.InvokeCliAsync("git", "rev-parse", "--abbrev-ref", "HEAD")).ReturnsAsync(PowerShellInvocationResultStubs.WithResults("main"));
		return target.Verifiable(
			ps => ps.InvokeCliAsync("git", It.Is<string[]>(args => VerifyCliArgs(args, expectedRepository))),
			s => s.ReturnsAsync(PowerShellInvocationResultStubs.Empty)
		);
	}

	static bool VerifyCliArgs(string[] args, string expectedRepository)
	{
		var nonSwitchArgs = args.Where(arg => !arg.StartsWith('-')).ToArray();
		return nonSwitchArgs.Length == 3 && nonSwitchArgs[0] == "clone" && nonSwitchArgs[1] == expectedRepository && nonSwitchArgs[2] == ".";
	}
}
