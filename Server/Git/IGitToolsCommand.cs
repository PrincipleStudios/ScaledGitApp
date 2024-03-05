using PrincipleStudios.ScaledGitApp.Commands;

namespace PrincipleStudios.ScaledGitApp.Git;

public interface IGitToolsCommand<out T> : ICommand<T, IGitToolsCommandContext> where T : Task
{
}

public interface IPowerShellCommand<out T> : ICommand<T, IPowerShellCommandContext> where T : Task
{
}
