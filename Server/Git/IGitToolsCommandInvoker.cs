namespace PrincipleStudios.ScaledGitApp.Git;

public interface IGitToolsCommandInvoker
{
	IPowerShellCommandInvoker PowerShellCommandInvoker { get; }

	Task RunCommand(IGitToolsCommand<Task> command);
	Task<T> RunCommand<T>(IGitToolsCommand<Task<T>> command);
}

public interface IPowerShellCommandInvoker
{
	Task RunCommand(IPowerShellCommand<Task> command);
	Task<T> RunCommand<T>(IPowerShellCommand<Task<T>> command);
}

internal static class GitToolsCommandInvokerExtensions
{
	public static Task RunCommand(this IGitToolsCommandInvoker target, IPowerShellCommand<Task> command) =>
		target.PowerShellCommandInvoker.RunCommand(command);
	public static Task<T> RunCommand<T>(this IGitToolsCommandInvoker target, IPowerShellCommand<Task<T>> command) =>
		target.PowerShellCommandInvoker.RunCommand<T>(command);
}