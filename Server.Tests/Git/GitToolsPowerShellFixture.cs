using Moq;
using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.Git;

public class GitToolsPowerShellFixture
{
	public GitOptions GitOptions { get; set; } = new GitOptions
	{
		WorkingDirectory = "./",
		GitToolsDirectory = "",
	};
	public Mock<IPowerShell> MockPowerShell { get; } = new Mock<IPowerShell>(MockBehavior.Strict);

	/// <param name="options">Uses `gitOptions` above if not provided</param>
	/// <returns>A GitToolsPowerShell instance</returns>
	public IGitToolsPowerShell Create(GitOptions? options = null)
	{
		GitOptions = options ?? GitOptions;

		return new GitToolsPowerShell(MockPowerShell.Object, GitOptions);
	}

}
