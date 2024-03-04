using Moq;
using PrincipleStudios.ScaledGitApp.ShellUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace PrincipleStudios.ScaledGitApp.Git;

public class GitCloneConfigurationFactoryShould
{
	private GitOptions defaultOptions;
	private string baseWorkingDirectory;
	private string expectedWorkingDirectory;
	private Mock<IPowerShell> mockFindGitRoot;
	private Mock<PowerShellFactory> mockPowerShellFactory;
	private readonly VerifiableMock<PowerShellFactory, IPowerShell> createPowershell;

	public GitCloneConfigurationFactoryShould()
	{
		defaultOptions = new GitOptions
		{
			WorkingDirectory = "./",
			GitToolsDirectory = "",
		};
		// These "real" directories are used to ensure file path separators, etc. do not break the test
		baseWorkingDirectory = Directory.GetCurrentDirectory();
		expectedWorkingDirectory = Path.TrimEndingDirectorySeparator(Path.GetTempPath());
		mockFindGitRoot = new Mock<IPowerShell>();
		mockPowerShellFactory = new Mock<PowerShellFactory>(MockBehavior.Strict);
		createPowershell = mockPowerShellFactory.Verifiable(ps => ps.Create(null), s => s.Returns(mockFindGitRoot.Object));
	}

	private GitCloneConfigurationFactory CreateTarget() =>
		new GitCloneConfigurationFactory(
			mockPowerShellFactory.Object,
			Options.Create(defaultOptions),
			Mock.Of<ILogger<GitCloneConfigurationFactory>>()
		);

	private void SetupBaseDirectory()
	{
		mockFindGitRoot.Setup(ps => ps.SetCurrentWorkingDirectory(baseWorkingDirectory));
		mockFindGitRoot.Setup(ps => ps.InvokeCliAsync("git", "rev-parse", "--show-toplevel"))
			.ReturnsAsync(PowerShellInvocationResultStubs.WithResults(expectedWorkingDirectory));
	}

	private static readonly string[] defaultRemotes = ["origin"];
	private void SetupRemotes(string[]? remotes = null)
	{
		remotes ??= defaultRemotes;
		mockFindGitRoot.Setup(ps => ps.InvokeCliAsync("git", "remote"))
			.ReturnsAsync(PowerShellInvocationResultStubs.WithResults(remotes));
	}

	private static readonly string[] defaultConfiguration = [
		"remote.origin.fetch=+refs/heads/*:refs/remotes/origin/*"
	];
	private void SetupConfiguration(string[]? configuration = null)
	{
		configuration ??= defaultConfiguration;
		mockFindGitRoot.Setup(ps => ps.InvokeCliAsync("git", "config", "--list"))
			.ReturnsAsync(PowerShellInvocationResultStubs.WithResults(configuration));
	}

	/// <summary>
	/// Verifies that the working directory is detected correctly
	/// </summary>
	[Fact]
	public async Task Provides_the_git_root_directory()
	{
		// Arrange
		SetupBaseDirectory();
		SetupRemotes();
		SetupConfiguration();
		var target = CreateTarget();

		// Act
		var result = await target.DetectGitCloneConfiguration();

		// Assert
		Assert.Equal(expectedWorkingDirectory, result.GitRootDirectory);
		createPowershell.Verify(Times.Once);
	}

	/// <summary>
	/// Verifies that mappings are loaded correctly
	/// </summary>
	[Fact]
	public async Task Parses_default_fetch_mappings()
	{
		// Arrange
		SetupBaseDirectory();
		SetupRemotes();
		SetupConfiguration();
		var target = CreateTarget();

		// Act
		var result = await target.DetectGitCloneConfiguration();

		// Assert
		Assert.Collection(result.FetchMapping,
			defaultMapping =>
			{
				Assert.True(defaultMapping.AllowNonFastForward);
				Assert.Equal("refs/remotes/origin/", defaultMapping.LocalMappingParts[0]);
				Assert.Equal(string.Empty, defaultMapping.LocalMappingParts[1]);
			});
		createPowershell.Verify(Times.Once);
	}

	/// <summary>
	/// Verifies that the scaled git config configuration is defaulted correctly
	/// </summary>
	[Fact]
	public async Task Passes_default_tool_configuration()
	{
		// Arrange
		SetupBaseDirectory();
		SetupRemotes();
		SetupConfiguration();
		var target = CreateTarget();

		// Act
		var result = await target.DetectGitCloneConfiguration();

		// Assert
		Assert.Equal("origin", result.RemoteName);
		Assert.Equal("_upstream", result.UpstreamBranchName);
	}

	/// <summary>
	/// Finds alternate remote mappings
	/// </summary>
	[Fact]
	public async Task Parses_alternate_remote_lists()
	{
		// Arrange
		SetupBaseDirectory();
		SetupRemotes(["azure", "origin"]);
		SetupConfiguration([
			"remote.azure.fetch=+refs/heads/*:refs/remotes/azure/*",
			"remote.origin.fetch=+refs/heads/*:refs/remotes/origin/*",
			"remote.origin.fetch=+refs/pull/*/head:refs/remotes/origin-pr/*"
		]);
		var target = CreateTarget();

		// Act
		var result = await target.DetectGitCloneConfiguration();

		// Assert
		Assert.Collection(result.FetchMapping,
			defaultMapping =>
			{
				Assert.True(defaultMapping.AllowNonFastForward);
				Assert.Equal("refs/remotes/azure/", defaultMapping.LocalMappingParts[0]);
				Assert.Equal(string.Empty, defaultMapping.LocalMappingParts[1]);
			});
		Assert.Equal("azure", result.RemoteName);
		Assert.Equal("_upstream", result.UpstreamBranchName);
	}

	/// <summary>
	/// Finds alternate remote mappings
	/// </summary>
	[Fact]
	public async Task Parses_multiple_fetch_refspecs()
	{
		// Arrange
		SetupBaseDirectory();
		SetupRemotes();
		SetupConfiguration([
			"remote.origin.fetch=+refs/heads/*:refs/remotes/origin/*",
			"remote.origin.fetch=+refs/pull/*/head:refs/remotes/origin-pr/*"
		]);
		var target = CreateTarget();

		// Act
		var result = await target.DetectGitCloneConfiguration();

		// Assert
		Assert.Collection(result.FetchMapping,
			defaultMapping =>
			{
				Assert.True(defaultMapping.AllowNonFastForward);
				Assert.Equal("refs/remotes/origin/", defaultMapping.LocalMappingParts[0]);
				Assert.Equal(string.Empty, defaultMapping.LocalMappingParts[1]);
			},
			defaultMapping =>
			{
				Assert.True(defaultMapping.AllowNonFastForward);
				Assert.Equal("refs/remotes/origin-pr/", defaultMapping.LocalMappingParts[0]);
				Assert.Equal(string.Empty, defaultMapping.LocalMappingParts[1]);
			});
		Assert.Equal("origin", result.RemoteName);
		Assert.Equal("_upstream", result.UpstreamBranchName);
	}
}
