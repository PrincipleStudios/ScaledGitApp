namespace PrincipleStudios.ScaledGitApp.Commands;

public interface ICommandCache
{
	void ClearAll();

	Task<T> GetCommandResultOrInvoke<T, TContext>(ICommand<Task<T>, TContext> command, Func<Task<T>> invoke);

	// TODO: some way to clear SOME commands? That may not be necessary for this
	// application, but if this becomes a more generalized framework, it should
	// be considered.
}
