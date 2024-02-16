﻿using Microsoft.Extensions.Logging;
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

	/// <param name="options">Uses `gitOptions` above if not provided</param>
	/// <param name="mockFactoryDirectly">Prevents detecting git directory or runspace setup if true</param>
	/// <returns>The target GitToolsPowerShellInvoker instance</returns>
	public GitToolsPowerShellInvoker CreateTarget(GitOptions? options = null, bool mockFactoryDirectly = true)
	{
		GitOptions = options ??= GitOptions;

		var gitToolsPowerShell = mockFactoryDirectly
			? new GitToolsPowerShellInvoker(Options.Create(options), () => MockPowerShellFactory.Object.Create(), Mock.Of<ILogger<GitToolsPowerShellInvoker>>())
			: new GitToolsPowerShellInvoker(Options.Create(options), MockPowerShellFactory.Object, Mock.Of<ILogger<GitToolsPowerShellInvoker>>());

		return gitToolsPowerShell;
	}

}
