namespace PrincipleStudios.ScaledGitApp.Commands;

public interface ICommandInvoker<out TContext>
{
	Task RunCommand(ICommand<Task, TContext> command);
	Task<T> RunCommand<T>(ICommand<Task<T>, TContext> command);
}
