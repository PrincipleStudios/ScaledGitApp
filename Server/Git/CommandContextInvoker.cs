using PrincipleStudios.ScaledGitApp.Environment;

namespace PrincipleStudios.ScaledGitApp.Git;

public static class CommandContextInvoker
{

	public static async Task<T> RunCommand<TArg, T>(Func<TArg, T> runCommand, TArg context, ILogger logger) where T : Task
	{
		if (runCommand.Target == null) throw new ArgumentException("Delegate Target must be a command", nameof(runCommand));

		using var activity = TracingHelper.StartActivity(runCommand.Target.GetType().Name);
		logger.RunningGitToolsPowerShellCommand(runCommand.Target.GetType().Name);
		var result = runCommand(context);
		// Ensure the command has been awaited before disposing the activity, but since we can't await something of type T, this has no return type.
		await result.ConfigureAwait(false);
		return result;
	}

}
