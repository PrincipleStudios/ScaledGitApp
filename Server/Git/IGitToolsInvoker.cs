namespace PrincipleStudios.ScaledGitApp.Git;

public interface IGitToolsInvoker
{
	Task RunCommand(IGitToolsCommand<Task> command);
	Task<T> RunCommand<T>(IGitToolsCommand<Task<T>> command);
}
