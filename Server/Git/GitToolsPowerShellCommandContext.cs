using PrincipleStudios.ScaledGitApp.ShellUtilities;
using System.Management.Automation;

namespace PrincipleStudios.ScaledGitApp.Git;

public sealed class GitToolsPowerShellCommandContext : IGitToolsPowerShellCommandContext
{
	private readonly IPowerShellInvoker pwsh;
	private readonly GitOptions gitOptions;
	private readonly GitCloneConfiguration gitCloneConfiguration;
	private readonly ILogger logger;

	public GitToolsPowerShellCommandContext(IPowerShellInvoker pwsh, GitOptions gitOptions, GitCloneConfiguration gitCloneConfiguration, ILogger logger)
	{
		this.pwsh = pwsh;
		this.gitOptions = gitOptions;
		this.gitCloneConfiguration = gitCloneConfiguration;
		this.logger = logger;
	}

	public string UpstreamBranchName => ToLocalTrackingBranchName(gitCloneConfiguration.UpstreamBranchName)
		?? throw new InvalidOperationException($"Upstream branch {gitCloneConfiguration.UpstreamBranchName} is not tracked. Ensure it is covered by the fetch refspec.");

	public string? ToLocalTrackingBranchName(string remoteBranchName)
	{
		var rawMapping = gitCloneConfiguration.FetchMapping.Select(m => m.TryApply(remoteBranchName, out var result) ? result : null).Where(v => v != null).FirstOrDefault();
		if (rawMapping != null) return rawMapping;

		var fullyQualified = GitConventions.ToFullyQualifiedBranchName(remoteBranchName);
		return gitCloneConfiguration.FetchMapping.Select(m => m.TryApply(fullyQualified, out var result) ? result : null).Where(v => v != null).FirstOrDefault();
	}

	public Task<PowerShellInvocationResult> InvokeGitToolsAsync(string relativeScriptName, Action<PowerShell>? addParameters = null) =>
		pwsh.InvokeExternalScriptAsync(ToAbsoluteScriptPath(relativeScriptName), addParameters);

	public Task<PowerShellInvocationResult> InvokeGitToolsAsync<T>(string relativeScriptName, PSDataCollection<T> input, Action<PowerShell>? addParameters = null) =>
		pwsh.InvokeExternalScriptAsync(ToAbsoluteScriptPath(relativeScriptName), input, addParameters);

	private string ToAbsoluteScriptPath(string relativeScriptName)
	{
		return Path.Join(gitOptions.GitToolsDirectory, relativeScriptName);
	}

	public async Task RunCommand(IGitToolsCommand<Task> command) =>
		await await RunGenericCommand(command);
	public async Task<T> RunCommand<T>(IGitToolsCommand<Task<T>> command) =>
		await await RunGenericCommand(command);
	private async Task<T> RunGenericCommand<T>(IGitToolsCommand<T> command) where T : Task =>
		await CommandContextInvoker.RunCommand(command.RunCommand, this, logger);

	public async Task RunCommand(IPowerShellCommand<Task> command) =>
		await await RunGenericCommand(command);
	public async Task<T> RunCommand<T>(IPowerShellCommand<Task<T>> command) =>
		await await RunGenericCommand(command);
	private async Task<T> RunGenericCommand<T>(IPowerShellCommand<T> command) where T : Task =>
		await CommandContextInvoker.RunCommand(command.RunCommand, new PowerShellCommandContext(pwsh, logger), logger);


	public async Task<PowerShellInvocationResult> InvokeCliAsync(string command, params string[] arguments) =>
		await pwsh.InvokeCliAsync(command, arguments);

	public async Task<PowerShellInvocationResult> InvokeCliAsync<T>(string command, PSDataCollection<T> input, IEnumerable<string> arguments) =>
		await pwsh.InvokeCliAsync(command, input, arguments);
}
