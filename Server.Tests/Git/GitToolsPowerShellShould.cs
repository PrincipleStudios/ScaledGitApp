using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.Git;

public class GitToolsPowerShellShould
{
	private const string expectedRepository = "https://example.com/.git";
	private GitOptions gitOptions = new GitOptions { Repository = expectedRepository };
	private Mock<PowerShellFactory>? mockPowerShellFactory;

	GitToolsPowerShell CreateTarget(GitOptions? options = null)
	{
		gitOptions = options ??= gitOptions;

		mockPowerShellFactory = new Mock<PowerShellFactory>(MockBehavior.Strict);
		var gitToolsPowerShell = new GitToolsPowerShell(Options.Create(options), mockPowerShellFactory.Object, Mock.Of<ILogger<GitToolsPowerShell>>());

		return gitToolsPowerShell;
	}

	[Fact]
	public async Task Instantiate_without_immediately_invoking()
	{
		using var target = CreateTarget();

		await Task.Yield();
	}
}
