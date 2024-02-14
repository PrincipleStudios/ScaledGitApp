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

public class GitToolsPowerShellFixture
{
	public GitOptions GitOptions { get; set; } = new GitOptions
	{
		WorkingDirectory = "./",
		GitToolsDirectory = "",
	};
	public Mock<PowerShellFactory> MockPowerShellFactory { get; } = new Mock<PowerShellFactory>(MockBehavior.Strict);

	/// <param name="options">Uses `gitOptions` above if not provided</param>
	/// <param name="mockFactoryDirectly">Prevents detecting git directory or runspace setup if true</param>
	/// <returns>The target GitToolsPowerShell instance</returns>
	public GitToolsPowerShell CreateTarget(GitOptions? options = null, bool mockFactoryDirectly = true)
	{
		GitOptions = options ??= GitOptions;

		var gitToolsPowerShell = mockFactoryDirectly
			? new GitToolsPowerShell(Options.Create(options), () => MockPowerShellFactory.Object.Create(), Mock.Of<ILogger<GitToolsPowerShell>>())
			: new GitToolsPowerShell(Options.Create(options), MockPowerShellFactory.Object, Mock.Of<ILogger<GitToolsPowerShell>>());

		return gitToolsPowerShell;
	}

}
