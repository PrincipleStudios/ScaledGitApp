using Microsoft.Extensions.Logging;
using Moq;
using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.Git;

public class GitToolsFixture
{
	public Mock<IPowerShell> MockPowerShell { get; } = new Mock<IPowerShell>(MockBehavior.Strict);
	public Mock<IGitToolsInvoker> MockGitToolsInvoker { get; } = new Mock<IGitToolsInvoker>(MockBehavior.Strict);
	public GitCloneConfiguration CloneConfiguration { get; set; } = Defaults.DefaultCloneConfiguration;
	public Mock<IPowerShellCommandInvoker> PowerShellCommandInvoker { get; } = new Mock<IPowerShellCommandInvoker>(MockBehavior.Strict);
	public Mock<IGitToolsCommandInvoker> GitToolsCommandInvoker { get; } = new Mock<IGitToolsCommandInvoker>(MockBehavior.Strict);

	/// <param name="cloneConfiguration">Uses `CloneConfiguration` above if not provided</param>
	/// <returns>A GitToolsPowerShell instance</returns>
	public IGitToolsCommandContext Create(GitCloneConfiguration? cloneConfiguration = null)
	{
		CloneConfiguration = cloneConfiguration ?? CloneConfiguration;
		MockPowerShell.Setup(pwsh => pwsh.SetCurrentWorkingDirectory(CloneConfiguration.GitRootDirectory));

		var result = new Mock<IGitToolsCommandContext>();
		result.SetupGet(c => c.PowerShellInvoker).Returns(MockPowerShell.Object);
		result.SetupGet(c => c.GitToolsInvoker).Returns(MockGitToolsInvoker.Object);
		result.SetupGet(c => c.GitToolsCommandInvoker).Returns(GitToolsCommandInvoker.Object);
		result.SetupGet(c => c.PowerShellCommandInvoker).Returns(PowerShellCommandInvoker.Object);
		result.SetupGet(c => c.GitCloneConfiguration).Returns(CloneConfiguration);

		return result.Object;
	}
}
