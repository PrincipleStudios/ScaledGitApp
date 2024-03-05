using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.Git;

public interface IGitToolsCommandContext : IPowerShellCommandContext
{
	IGitToolsCommandInvoker GitToolsCommandInvoker { get; }
	IGitToolsInvoker GitToolsInvoker { get; }

	string UpstreamBranchName { get; }
	string? ToLocalTrackingBranchName(string remoteBranchName);
}

public interface IPowerShellCommandContext
{
	IPowerShellCommandInvoker PowerShellCommandInvoker { get; }
	IPowerShellInvoker PowerShellInvoker { get; }
}
