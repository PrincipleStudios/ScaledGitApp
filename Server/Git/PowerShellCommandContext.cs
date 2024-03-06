using PrincipleStudios.ScaledGitApp.Commands;
using PrincipleStudios.ScaledGitApp.Environment;
using PrincipleStudios.ScaledGitApp.ShellUtilities;
using System.Management.Automation;

namespace PrincipleStudios.ScaledGitApp.Git;

public sealed class PowerShellCommandContext(IPowerShellInvoker powerShell, ILogger logger) : IPowerShellCommandContext
{
	public IPowerShellCommandInvoker PowerShellCommandInvoker => new InstanceCommandInvoker<IPowerShellCommandContext>(this, logger);
	public IPowerShellInvoker PowerShellInvoker => powerShell;
}