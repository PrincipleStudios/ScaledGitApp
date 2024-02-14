using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.Git;

public class GitException : Exception
{
	public GitException() { }
	public GitException(string message) : base(message) { }
	public GitException(string message, Exception inner) : base(message, inner) { }

	public static GitException From(PowerShellInvocationResult response)
	{
		return new GitException(string.Join('\n', response.Streams.Error.Select(error => error.ToString())));
	}
}
