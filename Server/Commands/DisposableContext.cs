namespace PrincipleStudios.ScaledGitApp.Commands;

public sealed record DisposableContext<TContext>(TContext Context, IDisposable Disposable) : IDisposable
{
	public void Dispose()
	{
		Disposable.Dispose();
	}
}
