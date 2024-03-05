namespace PrincipleStudios.ScaledGitApp.Commands;

public class InstanceCommandInvoker<TContext>(TContext contextInstance, ILogger logger) : ICommandInvoker<TContext>
{
	public async Task RunCommand(ICommand<Task, TContext> command) =>
		await await RunGenericCommand(command);
	public async Task<T> RunCommand<T>(ICommand<Task<T>, TContext> command) =>
		await await RunGenericCommand(command);
	private async Task<T> RunGenericCommand<T>(ICommand<T, TContext> command) where T : Task =>
		await CommandContextInvoker.RunCommand(command, contextInstance, logger);
}
