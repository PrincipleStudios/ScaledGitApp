namespace PrincipleStudios.ScaledGitApp.Git;

public interface IGitToolsCommandInvoker : IPowerShellCommandInvoker
{
	Task RunCommand(IGitToolsCommand<Task> command);
	Task<T> RunCommand<T>(IGitToolsCommand<Task<T>> command);
}

public interface IPowerShellCommandInvoker
{
	Task RunCommand(IPowerShellCommand<Task> command);
	Task<T> RunCommand<T>(IPowerShellCommand<Task<T>> command);
}
