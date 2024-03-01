using Moq;
using PrincipleStudios.ScaledGitApp.ShellUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace PrincipleStudios.ScaledGitApp.Git;

public class GitCloneConfigurationFactoryShould
{

	/// <summary>
	/// Verifies that the working directory is detected correctly and is assigned to the final powershell instance
	/// </summary>
	[Fact]
	public async Task Adjusts_the_working_directory_to_the_git_root()
	{
		var defaultOptions = new GitOptions
		{
			WorkingDirectory = "./",
			GitToolsDirectory = "",
		};
		// These "real" directories are used to ensure file path separators, etc. do not break the test
		var baseWorkingDirectory = Directory.GetCurrentDirectory();
		var expectedWorkingDirectory = Path.TrimEndingDirectorySeparator(Path.GetTempPath());
		var mockFindGitRoot = new Mock<IPowerShell>();
		var mockFinal = new Mock<IPowerShell>();
		var mockPowerShellFactory = new Mock<PowerShellFactory>(MockBehavior.Strict);

		var createPowershell = mockPowerShellFactory.VerifiableSequence(
			ps => ps.Create(null),
			s => s.Returns(mockFindGitRoot.Object).Returns(mockFinal.Object)
		);
		// The following setups are how we find the expected working directory
		mockFindGitRoot.Setup(ps => ps.SetCurrentWorkingDirectory(baseWorkingDirectory));
		mockFindGitRoot.Setup(ps => ps.InvokeCliAsync("git", "rev-parse", "--show-toplevel"))
			.ReturnsAsync(PowerShellInvocationResultStubs.WithResults(expectedWorkingDirectory));
		// Set up so we can verify that the expected working directory is set
		var workingDirectorySet = mockFinal.Verifiable(ps => ps.SetCurrentWorkingDirectory(expectedWorkingDirectory));

		// By not mocking the factory, we test the typical DI constructor with working directory setup
		var target = new GitCloneConfigurationFactory(mockPowerShellFactory.Object, Options.Create(defaultOptions), Mock.Of<ILogger<GitCloneConfigurationFactory>>());

		var result = await target.DetectGitCloneConfiguration();

		// Runs once to get the toplevel
		Assert.Equal(expectedWorkingDirectory, result.GitRootDirectory);
		createPowershell.Verify(Times.Once);
	}

}
