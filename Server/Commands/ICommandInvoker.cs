namespace PrincipleStudios.ScaledGitApp.Commands;

public interface ICommandInvoker<out TContext>
{
	Task RunCommand(ICommand<Task, TContext> command);
	Task<T> RunCommand<T>(ICommand<Task<T>, TContext> command);
}

public abstract class CommandInvoker<TContext>(ILogger logger) : ICommandInvoker<TContext>
{
	public virtual async Task RunCommand(ICommand<Task, TContext> command) =>
		await await RunGenericCommand(command);
	public virtual async Task<T> RunCommand<T>(ICommand<Task<T>, TContext> command) =>
		await await RunGenericCommand(command);
	protected abstract Task<T> RunGenericCommand<T>(ICommand<T, TContext> command) where T : Task;

	protected async Task<T> RunGenericCommand<T>(ICommand<T, TContext> command, TContext context) where T : Task =>
		await CommandContextInvoker.RunCommand(command, context, logger);
}
