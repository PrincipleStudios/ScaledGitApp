namespace PrincipleStudios.ScaledGitApp.Git;

public record GitOptions
{
	/// <summary>
	/// The Git URL (https://git-scm.com/docs/git-clone#_git_urls) of the repository to clone.
	/// </summary>
	public string? Repository { get; init; }
	/// <summary>
	/// The working directory within which to run git commands. Relative paths will be based on the application's working directory.
	/// </summary>
	public string WorkingDirectory { get; init; } = "./";
	/// <summary>
	/// The path to the directory containing the Scalable Git Branching Tools (https://github.com/PrincipleStudios/scalable-git-branching-tools)
	/// </summary>
	public string? GitToolsDirectory { get; init; }
}
