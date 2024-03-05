using Moq;
using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

public class GitCloneShould
{
	private readonly PowerShellFixture fixture = new();

	[Fact]
	public async Task Issue_a_clone()
	{
		var expectedRepository = "https://example.com/.git";

		var verifyGitClone = SetupGitClone(fixture.MockPowerShell, expectedRepository);
		var target = new GitClone(expectedRepository);

		await target.RunCommand(fixture.Create());

		verifyGitClone.Verify(Times.Once);
	}

	internal static VerifiableMock<IPowerShellCommandContext, Task<PowerShellInvocationResult>> SetupGitClone(Mock<IPowerShellCommandContext> target, string expectedRepository)
	{
		// This is very permissive right now; clone should be bare, and then we don't need the other setups
		target.Setup(ps => ps.PowerShellInvoker.InvokeCliAsync("git", It.IsAny<string[]>())).ReturnsAsync(PowerShellInvocationResultStubs.Empty);
		target.Setup(ps => ps.PowerShellInvoker.InvokeCliAsync("git", "rev-parse", "--abbrev-ref", "HEAD")).ReturnsAsync(PowerShellInvocationResultStubs.WithResults("main"));
		return target.Verifiable(
			ps => ps.PowerShellInvoker.InvokeCliAsync("git", It.Is<string[]>(args => VerifyCliArgs(args, expectedRepository))),
			s => s.ReturnsAsync(PowerShellInvocationResultStubs.Empty)
		);
	}

	/// <summary>
	/// This function verifies that a set of args matches a `git clone
	/// <paramref name="expectedRepository"/>.` command with any other
	/// switches. At this time, it doesn't support parameterized args, but none
	/// are passed at this time, either. Tests will fail if this changes.
	/// </summary>
	/// <param name="args">The actual args from this command</param>
	/// <param name="expectedRepository">The repository we expect to clone</param>
	/// <returns>True if <paramref name="expectedRepository"/> is a `clone` command and it clones to the current directory</returns>
	static bool VerifyCliArgs(string[] args, string expectedRepository)
	{
		var nonSwitchArgs = args.Where(arg => !arg.StartsWith('-')).ToArray();
		return nonSwitchArgs.Length == 3 && nonSwitchArgs[0] == "clone" && nonSwitchArgs[1] == expectedRepository && nonSwitchArgs[2] == ".";
	}
}
