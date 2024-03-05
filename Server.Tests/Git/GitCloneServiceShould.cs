using Moq;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

namespace PrincipleStudios.ScaledGitApp.Git;

public class GitCloneServiceShould
{
	private const string expectedRepository = "https://example.com/.git";
	private const string unexpectedRepository = "https://example.com/2.git";
	private static readonly string expectedTopLevelDirectory = Path.TrimEndingDirectorySeparator(Path.GetTempPath());
	private static readonly InvalidOperationException stubUnknownException = new("Unknown exception");
	private static readonly GitException stubException = new();

	static (GitCloneService Service, Mock<IPowerShellCommandInvoker> PowerShellMock) CreateService(GitOptions? options = null)
	{
		options ??= new GitOptions { Repository = expectedRepository };

		var mockPowerShell = new Mock<IPowerShellCommandInvoker>(MockBehavior.Strict);
		var gitCloneService = new GitCloneService(Options.Create(options), mockPowerShell.Object, Mock.Of<ILogger<GitCloneService>>());

		return (gitCloneService, mockPowerShell);
	}

	[Fact]
	public async Task Does_nothing_if_no_repository_is_configured()
	{
		var (service, mockPwsh) = CreateService(new GitOptions { Repository = null });

		var (actual, remotes) = await service.EnsureGitClone();

		Assert.Equal(GitCloneServiceStatus.NoRepository, actual);
		// May be null or not, does not need to be enforced here
	}

	[Fact]
	public async Task Detects_if_no_remotes_are_found()
	{
		var (service, mockPwsh) = CreateService();
		SetupGitRemotes(mockPwsh, []);

		var (actual, remotes) = await service.EnsureGitClone();

		Assert.Equal(GitCloneServiceStatus.NoRemotes, actual);
		Assert.NotNull(remotes);
		Assert.Empty(remotes.Remotes);
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

		var (actual, remotes) = await service.EnsureGitClone();

		Assert.Equal(GitCloneServiceStatus.MultipleRemotes, actual);
		Assert.NotNull(remotes);
		Assert.Collection(remotes.Remotes,
			e => Assert.Equal(new GitRemoteEntry("github", expectedRepository), e),
			e => Assert.Equal(new GitRemoteEntry("azdo", unexpectedRepository), e)
		);
	}

	[Fact]
	public async Task Detects_repository_mismatch()
	{
		var (service, mockPwsh) = CreateService();
		SetupGitRemotes(mockPwsh, [new GitRemoteEntry("github", unexpectedRepository)]);

		var (actual, remotes) = await service.EnsureGitClone();

		Assert.Equal(GitCloneServiceStatus.RepositoryMismatch, actual);
		Assert.NotNull(remotes);
		Assert.Collection(remotes.Remotes,
			e => Assert.Equal(new GitRemoteEntry("github", unexpectedRepository), e)
		);
	}

	[Fact]
	public async Task Reports_existing_clone()
	{
		var (service, mockPwsh) = CreateService();
		SetupGitRemotes(mockPwsh, [new GitRemoteEntry("github", expectedRepository)]);

		var (actual, remotes) = await service.EnsureGitClone();

		Assert.Equal(GitCloneServiceStatus.AlreadyCloned, actual);
		Assert.NotNull(remotes);
		Assert.Collection(remotes.Remotes,
			e => Assert.Equal(new GitRemoteEntry("github", expectedRepository), e)
		);
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

		var (actual, remotes) = await service.EnsureGitClone();

		Assert.Equal(GitCloneServiceStatus.ClonedSuccessfully, actual);
		// remotes may or may not be null here; probably null, but is not needed to be enforced
	}

	[Fact]
	public async Task Reports_failed_clone()
	{
		var (service, mockPwsh) = CreateService();
		SetupNoGitDirectory(mockPwsh);
		mockPwsh.Setup(pwsh => pwsh.RunCommand(new GitClone(expectedRepository))).ThrowsAsync(stubException);

		var (actual, remotes) = await service.EnsureGitClone();

		Assert.Equal(GitCloneServiceStatus.CloneFailed, actual);
		Assert.Null(remotes);
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

	[Fact]
	public async Task Loads_default_configuration_values_for_new_clone()
	{
		string expectedUpstream = $"refs/remotes/{Defaults.DefaultCloneConfiguration.RemoteName}/{Defaults.DefaultCloneConfiguration.BaseUpstreamBranchName}";
		var (service, mockPwsh) = CreateService();
		mockPwsh
			.SetupSequence(pwsh => pwsh.RunCommand(It.IsAny<GitRemote>()))
			.ThrowsAsync(stubException)
			.ReturnsAsync(new GitRemoteResult([new GitRemoteEntry("origin", expectedRepository)]));
		mockPwsh.Setup(pwsh => pwsh.RunCommand(new GitClone(expectedRepository))).Returns(Task.CompletedTask);
		SetupResolveTopLevelDirectory(mockPwsh);
		SetupStandardConfiguration(mockPwsh);

		await service.DetectCloneConfiguration();

		Assert.True(service.DetectedConfigurationTask.IsCompleted);
		var actualConfig = await service.DetectedConfigurationTask;
		Assert.Equal(expectedTopLevelDirectory, actualConfig.GitRootDirectory);
		Assert.Equal(expectedUpstream, actualConfig.UpstreamBranchName);
		Assert.Collection(actualConfig.FetchMapping,
			refspec =>
			{
				Assert.True(refspec.TryApply("refs/heads/_upstream", out var output));
				Assert.Equal("refs/remotes/origin/_upstream", output);
			});
	}

	[Fact]
	public async Task Loads_custom_configuration_values_for_unknown_repository()
	{
		const string configuredUpstream = "my-upstream";
		const string expectedRemote = "github";
		string expectedUpstream = $"refs/remotes/{expectedRemote}/{configuredUpstream}";
		var (service, mockPwsh) = CreateService(new GitOptions { Repository = null });
		SetupGitRemotes(mockPwsh, [
			new GitRemoteEntry("azdo", expectedRepository),
			new GitRemoteEntry(expectedRemote, expectedRepository),
		]);
		SetupResolveTopLevelDirectory(mockPwsh);
		SetupCustomConfiguration(mockPwsh, [
			new($"remote.{expectedRemote}.fetch", [$"+refs/heads/*:refs/remotes/{expectedRemote}/*", $"+refs/pull/*/head:refs/remotes/prs/*"]),
			new("remote.azdo.fetch", ["+refs/heads/*:refs/remotes/azdo/*"]),
			new("scaled-git.remote", [expectedRemote]),
			new("scaled-git.upstreambranch", [configuredUpstream]),
		]);

		await service.DetectCloneConfiguration();

		Assert.True(service.DetectedConfigurationTask.IsCompleted);
		var actualConfig = await service.DetectedConfigurationTask;
		Assert.Equal(expectedTopLevelDirectory, actualConfig.GitRootDirectory);
		Assert.Equal(expectedRemote, actualConfig.RemoteName);
		Assert.Equal(expectedUpstream, actualConfig.UpstreamBranchName);
		Assert.Collection(actualConfig.FetchMapping,
			refspec =>
			{
				Assert.True(refspec.TryApply("refs/heads/_upstream", out var output));
				Assert.Equal($"refs/remotes/{expectedRemote}/_upstream", output);
			},
			refspec =>
			{
				Assert.True(refspec.TryApply("refs/pull/100/head", out var output));
				Assert.Equal($"refs/remotes/prs/100", output);
			});
	}

	[Fact]
	public void Provides_a_pending_task_for_configuration()
	{
		var (service, mockPwsh) = CreateService();

		Assert.False(service.DetectedConfigurationTask.IsCompleted);
	}

	private static void SetupGitRemotes(Mock<IPowerShellCommandInvoker> mockPwsh, GitRemoteEntry[] remotes)
	{
		mockPwsh
			.Setup(pwsh => pwsh.RunCommand(It.IsAny<GitRemote>()))
			.ReturnsAsync(new GitRemoteResult(remotes));
	}
	private static void SetupNoGitDirectory(Mock<IPowerShellCommandInvoker> mockPwsh)
	{
		mockPwsh
			.Setup(pwsh => pwsh.RunCommand(It.IsAny<GitRemote>()))
			.ThrowsAsync(stubException);
	}

	private static void SetupResolveTopLevelDirectory(Mock<IPowerShellCommandInvoker> mockPwsh)
	{
		mockPwsh
			.Setup(pwsh => pwsh.RunCommand(It.IsAny<ResolveTopLevelDirectory>()))
			.ReturnsAsync(expectedTopLevelDirectory);
	}

	private static void SetupStandardConfiguration(Mock<IPowerShellCommandInvoker> mockPwsh)
	{
		mockPwsh
			.Setup(pwsh => pwsh.RunCommand(It.IsAny<GitConfigurationList>()))
			.ReturnsAsync(new Dictionary<string, IReadOnlyList<string>>
			{
				{ "remote.origin.fetch", ["+refs/heads/*:refs/remotes/origin/*"] },
			});
	}

	private static void SetupCustomConfiguration(Mock<IPowerShellCommandInvoker> mockPwsh, params KeyValuePair<string, IReadOnlyList<string>>[] items)
	{
		mockPwsh
			.Setup(pwsh => pwsh.RunCommand(It.IsAny<GitConfigurationList>()))
			.ReturnsAsync(new Dictionary<string, IReadOnlyList<string>>(items));
	}

}
