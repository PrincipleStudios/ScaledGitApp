using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PrincipleStudios.ScaledGitApp.ShellUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrincipleStudios.ScaledGitApp.Git;

public class GitToolsPowerShellInvokerFixture
{
	public GitOptions GitOptions { get; set; } = new GitOptions
	{
		WorkingDirectory = "./",
		GitToolsDirectory = "",
	};
	public Mock<PowerShellFactory> MockPowerShellFactory { get; } = new Mock<PowerShellFactory>(MockBehavior.Strict);
	public GitCloneConfiguration CloneConfiguration { get; set; } = Defaults.DefaultCloneConfiguration;

	/// <param name="options">Uses `gitOptions` above if not provided</param>
	/// <param name="mockFactoryDirectly">Prevents detecting git directory or runspace setup if true</param>
	/// <returns>The target GitToolsPowerShellInvoker instance</returns>
	public GitToolsCommandInvoker CreateTarget(GitOptions? options = null, GitCloneConfiguration? cloneConfiguration = null)
	{
		GitOptions = options ?? GitOptions;
		CloneConfiguration = cloneConfiguration ?? CloneConfiguration;

		var gitToolsPowerShell = new GitToolsCommandInvoker(
			Options.Create(GitOptions),
			MockPowerShellFactory.Object,
			CloneConfiguration,
			Mock.Of<ILogger<GitToolsCommandInvoker>>()
		);

		return gitToolsPowerShell;
	}

}
