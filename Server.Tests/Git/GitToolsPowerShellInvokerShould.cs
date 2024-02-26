using Moq;
using PrincipleStudios.ScaledGitApp.ShellUtilities;
using PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

namespace PrincipleStudios.ScaledGitApp.Git;

public partial class GitToolsPowerShellInvokerShould
{
	private readonly GitToolsPowerShellInvokerFixture fixture = new GitToolsPowerShellInvokerFixture();

	[Fact]
	public async Task Instantiate_without_immediately_invoking()
	{
		fixture.MockPowerShellFactory.Setup(ps => ps.Create(null)).Returns((IPowerShell)null!);

		// By not mocking the factory, we test the typical DI constructor with working directory setup
		var target = fixture.CreateTarget(mockFactoryDirectly: false);

		await Task.Yield();

		fixture.MockPowerShellFactory.Verify(ps => ps.Create(null), Times.Never());
	}

	/// <summary>
	/// Verifies that the working directory is detected correctly and is assigned to the final powershell instance
	/// </summary>
	[Fact]
	public async Task Adjusts_the_working_directory_to_the_git_root()
	{
		// These "real" directories are used to ensure file path separators, etc. do not break the test
		var baseWorkingDirectory = Directory.GetCurrentDirectory();
		var expectedWorkingDirectory = Path.TrimEndingDirectorySeparator(Path.GetTempPath());
		var mockFindGitRoot = new Mock<IPowerShell>();
		var mockFinal = new Mock<IPowerShell>();

		// The first time `.Create` is called, it returns the instance that will find the root.
		// The second time, it is the instance used to run the command
		var createPowershell = fixture.MockPowerShellFactory.VerifiableSequence(
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
		var target = fixture.CreateTarget(mockFactoryDirectly: false);

		await target.RunCommand(Mock.Of<IGitToolsCommand<Task>>());

		// Runs once to get the toplevel 
		createPowershell.Verify(Times.Exactly(2));
		workingDirectorySet.Verify(Times.Once);
	}

	/// <summary>
	/// Verifies that the actual command with the mock that was passed
	/// </summary>
	[Fact]
	public async Task Executes_the_target_command()
	{
		var expectedResult = "foo";
		var mockFinal = new Mock<IPowerShell>();
		fixture.MockPowerShellFactory.Setup(ps => ps.Create(null)).Returns(mockFinal.Object);

		var mockCommand = new Mock<IGitToolsCommand<Task<string>>>();
		var verifiableCommand = mockCommand.Verifiable(cmd => cmd.RunCommand(It.IsAny<IGitToolsPowerShell>()), s => s.ReturnsAsync(expectedResult));

		// By mocking the factory directly, we skip working directory detection
		var target = fixture.CreateTarget();

		var result = await target.RunCommand(mockCommand.Object);

		// Assert that we got the expected result from the command because the value was passed through
		Assert.Equal(expectedResult, result);
		verifiableCommand.Verify(Times.Once);
	}
}
