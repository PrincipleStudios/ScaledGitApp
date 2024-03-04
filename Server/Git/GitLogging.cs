namespace PrincipleStudios.ScaledGitApp.Git;

public static partial class GitLogging
{
	[LoggerMessage(LogLevel.Information, "No git repository configured; skipping clone.")]
	public static partial void NoGitRepositoryConfigured(this ILogger logger);

	[LoggerMessage(LogLevel.Information, "Running {CommandType}.")]
	public static partial void RunningGitToolsPowerShellCommand(this ILogger logger, string commandType);


	[LoggerMessage(LogLevel.Information, "Git was already cloned in {Directory} for {Remote}")]
	public static partial void GitAlreadyCloned(this ILogger logger, string remote, string directory);

	[LoggerMessage(LogLevel.Information, "Git successfully cloned in {Directory} for {Remote}")]
	public static partial void GitClonedSuccessfully(this ILogger logger, string remote, string directory);

	[LoggerMessage(LogLevel.Error, "Git already initialized with no remotes")]
	public static partial void GitWithNoRemotes(this ILogger logger);

	[LoggerMessage(LogLevel.Warning, "Multiple git repositories were configured: {GitRepositories}. Use a separate directory for this application for expected behavior.")]
	public static partial void MultipleGitRepositoriesConfigured(this ILogger logger, IEnumerable<string> gitRepositories);

	[LoggerMessage(LogLevel.Warning, "Expected git repository ({Expected}) did not match the already-configured remote ({Actual})")]
	public static partial void GitRepositoryMismatch(this ILogger logger, string? expected, string actual);

	[LoggerMessage(LogLevel.Error, "Git encountered an error while cloning the repository.")]
	public static partial void GitFailedToClone(this ILogger logger, GitException exception);

	[LoggerMessage(LogLevel.Critical, "Unknown exception while setting up the git clone.")]
	public static partial void GitFailedToCloneWithUnknownException(this ILogger logger, Exception exception);
}
