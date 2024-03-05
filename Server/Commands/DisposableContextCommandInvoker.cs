namespace PrincipleStudios.ScaledGitApp.Commands;

public class DisposableContextCommandInvoker<TContext>(Func<Task<DisposableContext<TContext>>> constructor, ILogger logger) : ICommandInvoker<TContext>
{
	public async Task RunCommand(ICommand<Task, TContext> command) =>
		await await RunGenericCommand(command);
	public async Task<T> RunCommand<T>(ICommand<Task<T>, TContext> command) =>
		await await RunGenericCommand(command);
	private async Task<T> RunGenericCommand<T>(ICommand<T, TContext> command) where T : Task
	{
		using var result = await constructor();
		return await CommandContextInvoker.RunCommand(command, result.Context, logger);
	}
}
