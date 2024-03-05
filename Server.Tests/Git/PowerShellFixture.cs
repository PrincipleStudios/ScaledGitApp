using Moq;
using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.Git;

public class PowerShellFixture
{
	public Mock<IPowerShell> MockPowerShell { get; } = new Mock<IPowerShell>(MockBehavior.Strict);

	/// <returns>A IPowerShell instance</returns>
	public IPowerShell Create()
	{
		return MockPowerShell.Object;
	}

}
