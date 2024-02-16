using Moq;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

namespace PrincipleStudios.ScaledGitApp.Git;

public class GitCloneServiceShould
{
	private const string expectedRepository = "https://example.com/.git";
	private const string unexpectedRepository = "https://example.com/2.git";
	private static readonly InvalidOperationException stubUnknownException = new("Unknown exception");
	private static readonly GitException stubException = new();

	static (GitCloneService Service, Mock<IGitToolsInvoker> PowerShellMock) CreateService(GitOptions? options = null)
	{
		options ??= new GitOptions { Repository = expectedRepository };

		var mockPowerShell = new Mock<IGitToolsInvoker>(MockBehavior.Strict);
		var gitCloneService = new GitCloneService(Options.Create(options), mockPowerShell.Object, Mock.Of<ILogger<GitCloneService>>());

		return (gitCloneService, mockPowerShell);
	}

	[Fact]
	public async Task Does_nothing_if_no_repository_is_configured()
	{
		var (service, mockPwsh) = CreateService(new GitOptions { Repository = null });

		var actual = await service.EnsureGitClone();

		Assert.Equal(GitCloneServiceStatus.NoRepository, actual);
	}

	[Fact]
	public async Task Detects_if_no_remotes_are_found()
	{
		var (service, mockPwsh) = CreateService();
		SetupGitRemotes(mockPwsh, []);

		var actual = await service.EnsureGitClone();

		Assert.Equal(GitCloneServiceStatus.NoRemotes, actual);
	}

	[Fact]
	public async Task Detects_if_multiple_remotes_are_found()
	{
		var (service, mockPwsh) = CreateService();
		SetupGitRemotes(mockPwsh,
			[
				new GitRemoteEntry("github", expectedRepository),
				new GitRemoteEntry("azdo", unexpectedRepository),
			]);

		var actual = await service.EnsureGitClone();

		Assert.Equal(GitCloneServiceStatus.MultipleRemotes, actual);
	}

	[Fact]
	public async Task Detects_repository_mismatch()
	{
		var (service, mockPwsh) = CreateService();
		SetupGitRemotes(mockPwsh, [new GitRemoteEntry("github", unexpectedRepository)]);

		var actual = await service.EnsureGitClone();

		Assert.Equal(GitCloneServiceStatus.RepositoryMismatch, actual);
	}

	[Fact]
	public async Task Reports_existing_clone()
	{
		var (service, mockPwsh) = CreateService();
		SetupGitRemotes(mockPwsh, [new GitRemoteEntry("github", expectedRepository)]);

		var actual = await service.EnsureGitClone();

		Assert.Equal(GitCloneServiceStatus.AlreadyCloned, actual);
	}

	[Fact]
	public async Task Throws_if_remote_has_other_errors()
	{
		var (service, mockPwsh) = CreateService();
		mockPwsh.Setup(pwsh => pwsh.RunCommand(It.IsAny<GitRemote>())).ThrowsAsync(stubUnknownException);

		var actualException = await Assert.ThrowsAsync<InvalidOperationException>(service.EnsureGitClone);

		Assert.Equal(stubUnknownException, actualException);
	}

	[Fact]
	public async Task Requests_a_clone_if_nonexisting()
	{
		var (service, mockPwsh) = CreateService();
		SetupNoGitDirectory(mockPwsh);
		mockPwsh.Setup(pwsh => pwsh.RunCommand(new GitClone(expectedRepository))).Returns(Task.CompletedTask);

		var actual = await service.EnsureGitClone();

		Assert.Equal(GitCloneServiceStatus.ClonedSuccessfully, actual);
	}

	[Fact]
	public async Task Reports_failed_clone()
	{
		var (service, mockPwsh) = CreateService();
		SetupNoGitDirectory(mockPwsh);
		mockPwsh.Setup(pwsh => pwsh.RunCommand(new GitClone(expectedRepository))).ThrowsAsync(stubException);

		var actual = await service.EnsureGitClone();

		Assert.Equal(GitCloneServiceStatus.CloneFailed, actual);
	}

	[Fact]
	public async Task Throws_if_clone_had_other_errors()
	{
		var (service, mockPwsh) = CreateService();
		SetupNoGitDirectory(mockPwsh);
		mockPwsh.Setup(pwsh => pwsh.RunCommand(new GitClone(expectedRepository))).ThrowsAsync(stubUnknownException);

		var actualException = await Assert.ThrowsAsync<InvalidOperationException>(service.EnsureGitClone);

		Assert.Equal(stubUnknownException, actualException);
	}

	private static void SetupGitRemotes(Mock<IGitToolsInvoker> mockPwsh, GitRemoteEntry[] remotes)
	{
		mockPwsh
			.Setup(pwsh => pwsh.RunCommand(It.IsAny<GitRemote>()))
			.ReturnsAsync(new GitRemoteResult(remotes));
	}
	private static void SetupNoGitDirectory(Mock<IGitToolsInvoker> mockPwsh)
	{
		mockPwsh
			.Setup(pwsh => pwsh.RunCommand(It.IsAny<GitRemote>()))
			.ThrowsAsync(stubException);
	}
}
