﻿using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

/// <summary>
/// Clones a git repository into the git working directory.
/// </summary>
/// <param name="Repository">The Git URL for the repository to clone</param>
public record GitFetch() : IGitToolsCommand<Task>
{
	public async Task RunCommand(IGitToolsPowerShell pwsh)
	{
		(await pwsh.InvokeCliAsync("git", "fetch", "--porcelain")).ThrowIfHadErrors();
	}
}
