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

		// By mocking the factory directly, we test the typical DI constructor, which should not call immediately
		using var target = fixture.CreateTarget(mockFactoryDirectly: false);

		await Task.Yield();

		fixture.MockPowerShellFactory.Verify(ps => ps.Create(null), Times.Never());
	}

	[Fact]
	public async Task Adjusts_the_working_directory_to_the_git_root()
	{
		var expectedResult = "foo";
		var baseWorkingDirectory = Directory.GetCurrentDirectory();
		var expectedWorkingDirectory = Path.TrimEndingDirectorySeparator(Path.GetTempPath());
		var mockFindGitRoot = new Mock<IPowerShell>();
		using var runspace = System.Management.Automation.Runspaces.RunspaceFactory.CreateRunspace();
		var mockFinal = new Mock<IPowerShell>();
		runspace.Open();

		fixture.MockPowerShellFactory.Setup(ps => ps.Create(null)).Returns(mockFindGitRoot.Object);
		mockFindGitRoot.Setup(ps => ps.SetCurrentWorkingDirectory(baseWorkingDirectory));
		mockFindGitRoot.Setup(ps => ps.InvokeCliAsync("git", "rev-parse", "--show-toplevel"))
			.ReturnsAsync(PowerShellInvocationResultStubs.WithResults(expectedWorkingDirectory));
		fixture.MockPowerShellFactory.Setup(ps => ps.CreateRunspace(null)).Returns(runspace);
		var createdWithRunspace = fixture.MockPowerShellFactory.Verifiable(ps => ps.Create(runspace), s => s.Returns(mockFinal.Object));

		var mockCommand = new Mock<IGitToolsCommand<Task<string>>>();
		var verifiableCommand = mockCommand.Verifiable(cmd => cmd.RunCommand(It.IsAny<IGitToolsPowerShell>()), s => s.ReturnsAsync(expectedResult));

		// By mocking the factory directly, we test the typical DI constructor with working directory setup
		using var target = fixture.CreateTarget(mockFactoryDirectly: false);

		var result = await target.RunCommand(mockCommand.Object);

		Assert.Equal(expectedWorkingDirectory, Path.TrimEndingDirectorySeparator(runspace.SessionStateProxy.Path.CurrentLocation.Path));
		Assert.Equal(expectedResult, result);
		createdWithRunspace.Verify(Times.Once);
		verifiableCommand.Verify(Times.Once);
	}

	[Fact]
	public async Task Allows_bypassing_the_mock_factory()
	{
		var expectedResult = "foo";
		var mockFinal = new Mock<IPowerShell>();
		fixture.MockPowerShellFactory.Setup(ps => ps.Create(null)).Returns(mockFinal.Object);

		var mockCommand = new Mock<IGitToolsCommand<Task<string>>>();
		var verifiableCommand = mockCommand.Verifiable(cmd => cmd.RunCommand(It.IsAny<IGitToolsPowerShell>()), s => s.ReturnsAsync(expectedResult));

		// By mocking the factory directly, we test the typical DI constructor with working directory setup
		using var target = fixture.CreateTarget();

		var result = await target.RunCommand(mockCommand.Object);

		Assert.Equal(expectedResult, result);
		verifiableCommand.Verify(Times.Once);
	}
}
