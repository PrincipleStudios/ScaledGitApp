namespace PrincipleStudios.ScaledGitApp.Git;

public interface IGitToolsPowerShell : IDisposable
{
	/// <summary>
	/// Clones a git repository into the git working directory.
	/// </summary>
	/// <param name="repository">The Git URL for the repository to clone</param>
	Task GitClone(string repository);

	/// <summary>
	/// Fetches updates in the git working directory.
	/// </summary>
	Task GitFetch();

	/// <summary>
	/// Gets the list of remotes for the git working directory
	/// </summary>
	/// <returns>A list of remotes</returns>
	Task<GitRemoteResult> GitRemote();
}

public record GitRemoteResult(IReadOnlyList<GitRemote> Remotes);
public record GitRemote(string Alias, string FetchUrl);
