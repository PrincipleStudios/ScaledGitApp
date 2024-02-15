using PrincipleStudios.ScaledGitApp.Git;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace PrincipleStudios.ScaledGitApp.ShellUtilities;

public static class PowerShellExtensions
{
	public static void SetCurrentWorkingDirectory(this PowerShell shell, string workingDirectory) =>
		shell.Runspace.SetCurrentWorkingDirectory(workingDirectory);
	public static void SetCurrentWorkingDirectory(this Runspace runspace, string workingDirectory) =>
		runspace.SessionStateProxy.Path.SetLocation(workingDirectory);
}
