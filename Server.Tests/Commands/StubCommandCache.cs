namespace PrincipleStudios.ScaledGitApp.Commands;

public class StubCommandCache : ICommandCache
{
	public void ClearAll()
	{
	}

	public Task<T> GetCommandResultOrInvoke<T, TContext>(ICommand<Task<T>, TContext> command, Func<Task<T>> invoke)
	{
		return invoke();
	}
}
