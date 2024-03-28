using Microsoft.Extensions.Options;
using PrincipleStudios.ScaledGitApp.Commands;
using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.Git;

public sealed class PowerShellCommandInvoker(IOptions<GitOptions> options, PowerShellFactory psFactory, ILogger<PowerShellCommandInvoker> logger, ICommandCache commandCache)
	: CachingCommandInvoker<IPowerShellCommandContext>(logger, commandCache)
{
	protected override async Task<T> RunGenericCommand<T>(ICommand<T, IPowerShellCommandContext> command)
	{
		using var pwsh = psFactory.Create();
		pwsh.SetCurrentWorkingDirectory(options.Value.WorkingDirectory);
		var context = new PowerShellCommandContext(pwsh, logger, Cache);
		return await RunGenericCommand(command, context);
	}
}
