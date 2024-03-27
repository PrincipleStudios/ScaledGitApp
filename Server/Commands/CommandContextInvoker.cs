using PrincipleStudios.ScaledGitApp.Environment;
using PrincipleStudios.ScaledGitApp.Git;

namespace PrincipleStudios.ScaledGitApp.Commands;

public static class CommandContextInvoker
{

	public static async Task<TResult> RunCommand<TContext, TResult>(ICommand<TResult, TContext> command, TContext context, ILogger logger) where TResult : Task
	{
		using var activity = TracingHelper.StartActivity(command.GetType().Name);
		logger.RunningCommand(command.GetType().Name);
		var result = command.RunCommand(context);
		// Ensure the command has been awaited before disposing the activity, but since we can't await something of type T, this has no return type.
		await result.ConfigureAwait(false);
		return result;
	}

}
