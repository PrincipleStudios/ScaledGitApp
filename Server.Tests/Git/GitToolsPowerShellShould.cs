using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.Git;

public partial class GitToolsPowerShellShould
{
	private GitOptions gitOptions = new GitOptions
	{
		WorkingDirectory = "./",
		GitToolsDirectory = "",
	};
	private Mock<PowerShellFactory> mockPowerShellFactory = new Mock<PowerShellFactory>(MockBehavior.Strict);

	/// <param name="options">Uses `gitOptions` above if not provided</param>
	/// <param name="mockFactoryDirectly">Prevents detecting git directory or runspace setup if true</param>
	/// <returns>The target GitToolsPowerShell instance</returns>
	GitToolsPowerShell CreateTarget(GitOptions? options = null, bool mockFactoryDirectly = true)
	{
		gitOptions = options ??= gitOptions;

		var gitToolsPowerShell = mockFactoryDirectly
			? new GitToolsPowerShell(Options.Create(options), () => mockPowerShellFactory.Object.Create(), Mock.Of<ILogger<GitToolsPowerShell>>())
			: new GitToolsPowerShell(Options.Create(options), mockPowerShellFactory.Object, Mock.Of<ILogger<GitToolsPowerShell>>());

		return gitToolsPowerShell;
	}

	[Fact]
	public async Task Instantiate_without_immediately_invoking()
	{
		mockPowerShellFactory.Setup(ps => ps.Create(null)).Returns((IPowerShell)null!);

		// By mocking the factory directly, we test the typical DI constructor, which should not call immediately
		using var target = CreateTarget(mockFactoryDirectly: false);

		await Task.Yield();

		mockPowerShellFactory.Verify(ps => ps.Create(null), Times.Never());
	}

	[Fact]
	public async Task Adjusts_the_working_directory_to_the_git_root()
	{
		var baseWorkingDirectory = Directory.GetCurrentDirectory();
		var expectedWorkingDirectory = Path.TrimEndingDirectorySeparator(Path.GetTempPath());
		var mockFindGitRoot = new Mock<IPowerShell>();
		using var runspace = System.Management.Automation.Runspaces.RunspaceFactory.CreateRunspace();
		var mockFinal = new Mock<IPowerShell>();
		runspace.Open();

		mockPowerShellFactory.Setup(ps => ps.Create(null)).Returns(mockFindGitRoot.Object);
		mockFindGitRoot.Setup(ps => ps.SetCurrentWorkingDirectory(baseWorkingDirectory));
		mockFindGitRoot.Setup(ps => ps.InvokeCliAsync("git", "rev-parse", "--show-toplevel"))
			.ReturnsAsync(PowerShellInvocationResultStubs.WithResults(expectedWorkingDirectory));
		mockPowerShellFactory.Setup(ps => ps.CreateRunspace(null)).Returns(runspace);
		var createdWithRunspace = mockPowerShellFactory.Verifiable(ps => ps.Create(runspace), s => s.Returns(mockFinal.Object));

		var verifyGitRemote = SetupGitRemote(mockFinal);

		// By mocking the factory directly, we test the typical DI constructor with working directory setup
		using var target = CreateTarget(mockFactoryDirectly: false);

		var remotes = await target.GitRemote();

		Assert.Equal(expectedWorkingDirectory, Path.TrimEndingDirectorySeparator(runspace.SessionStateProxy.Path.CurrentLocation.Path));
		createdWithRunspace.Verify(Times.Once);
		verifyGitRemote.Verify(Times.Once);
	}

	[Fact]
	public async Task Allows_bypassing_the_mock_factory()
	{
		var mockFinal = new Mock<IPowerShell>();
		mockPowerShellFactory.Setup(ps => ps.Create(null)).Returns(mockFinal.Object);

		var verifyGitRemote = SetupGitRemote(mockFinal);

		// By mocking the factory directly, we test the typical DI constructor with working directory setup
		using var target = CreateTarget();

		var remotes = await target.GitRemote();

		verifyGitRemote.Verify(Times.Once);
	}

	internal static VerifiableMock<IPowerShell, Task<PowerShellInvocationResult>> SetupGitRemote(Mock<IPowerShell> target, IEnumerable<GitRemote>? remotes = null)
	{
		return target.Verifiable(ps => ps.InvokeCliAsync("git", "remote", "-v"), s => s.ReturnsAsync(PowerShellInvocationResultStubs.Empty));
	}
}
