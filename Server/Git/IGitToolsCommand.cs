using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.Git;

public interface IGitToolsCommand<T> where T : Task
{
	T RunCommand(IGitToolsPowerShell pwsh);
}

public interface IPowerShellCommand<T> where T : Task
{
	T RunCommand(IPowerShell pwsh);
}
