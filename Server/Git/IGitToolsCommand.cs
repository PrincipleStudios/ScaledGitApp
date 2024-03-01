namespace PrincipleStudios.ScaledGitApp.Git;

public interface IGitToolsCommand<T> where T : Task
{
	T RunCommand(IGitToolsPowerShell pwsh);
}
