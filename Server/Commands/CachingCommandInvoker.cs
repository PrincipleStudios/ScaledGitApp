namespace PrincipleStudios.ScaledGitApp.Commands;

public abstract class CachingCommandInvoker<TContext>(ILogger logger, ICommandCache cache) : CommandInvoker<TContext>(logger)
{
	protected ICommandCache Cache => cache;

	public override Task<T> RunCommand<T>(ICommand<Task<T>, TContext> command) =>
		cache.GetCommandResultOrInvoke(command, async () => await await RunGenericCommand(command));
}
