using Moq;
using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.Git;

public class PowerShellFixture
{
	public Mock<IPowerShellCommandContext> MockPowerShell { get; } = new Mock<IPowerShellCommandContext>(MockBehavior.Strict);

	/// <returns>A IPowerShell instance</returns>
	public IPowerShellCommandContext Create()
	{
		return MockPowerShell.Object;
	}

}
