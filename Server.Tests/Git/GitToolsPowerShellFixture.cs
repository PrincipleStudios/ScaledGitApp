﻿using Microsoft.Extensions.Logging;
using Moq;
using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.Git;

public class GitToolsPowerShellFixture
{
	public GitOptions GitOptions { get; set; } = new GitOptions
	{
		WorkingDirectory = "./",
		GitToolsDirectory = "",
	};
	public Mock<IPowerShell> MockPowerShell { get; } = new Mock<IPowerShell>(MockBehavior.Strict);
	public GitCloneConfiguration CloneConfiguration { get; set; } = Defaults.DefaultCloneConfiguration;

	/// <param name="options">Uses `gitOptions` above if not provided</param>
	/// <returns>A GitToolsPowerShell instance</returns>
	public IGitToolsCommandContext Create(GitOptions? options = null, GitCloneConfiguration? cloneConfiguration = null)
	{
		GitOptions = options ?? GitOptions;
		CloneConfiguration = cloneConfiguration ?? CloneConfiguration;
		MockPowerShell.Setup(pwsh => pwsh.SetCurrentWorkingDirectory(CloneConfiguration.GitRootDirectory));

		return new GitToolsCommandContext(MockPowerShell.Object, GitOptions, CloneConfiguration, Mock.Of<ILogger>());
	}

}
