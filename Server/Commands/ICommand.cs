namespace PrincipleStudios.ScaledGitApp.Commands;

public interface ICommand<out TResult, in TContext> where TResult : Task
{
	TResult Execute(TContext context);
}
