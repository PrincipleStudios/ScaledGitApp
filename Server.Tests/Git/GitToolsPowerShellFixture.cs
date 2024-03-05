using Microsoft.Extensions.Logging;
using Moq;
using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.Git;

public class GitToolsPowerShellFixture
{
	public Mock<IPowerShell> MockPowerShell { get; } = new Mock<IPowerShell>(MockBehavior.Strict);
	public Mock<IGitToolsInvoker> MockGitToolsInvoker { get; } = new Mock<IGitToolsInvoker>(MockBehavior.Strict);
	public GitCloneConfiguration CloneConfiguration { get; set; } = Defaults.DefaultCloneConfiguration;

	/// <param name="cloneConfiguration">Uses `CloneConfiguration` above if not provided</param>
	/// <returns>A GitToolsPowerShell instance</returns>
	public IGitToolsCommandContext Create(GitCloneConfiguration? cloneConfiguration = null)
	{
		CloneConfiguration = cloneConfiguration ?? CloneConfiguration;
		MockPowerShell.Setup(pwsh => pwsh.SetCurrentWorkingDirectory(CloneConfiguration.GitRootDirectory));

		return new GitToolsCommandContext(MockPowerShell.Object, MockGitToolsInvoker.Object, CloneConfiguration, Mock.Of<ILogger>());
	}
}
