using Microsoft.Extensions.Options;
using PrincipleStudios.ScaledGitApp.ShellUtililties;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace PrincipleStudios.ScaledGitApp.Git;

public sealed class GitToolsPowershell : IDisposable
{
	private readonly GitOptions gitOptions;
	private readonly Lazy<Task<Runspace>> runspaceFactory;
	private readonly PowerShellFactory psFactory;
	private Runspace? runspace;
	private bool disposedValue;

	public GitToolsPowershell(IOptions<GitOptions> options, PowerShellFactory psFactory)
	{
		gitOptions = options.Value;
		runspaceFactory = new Lazy<Task<Runspace>>(CreateRunspace);
		this.psFactory = psFactory;
	}

	async Task<Runspace> CreateRunspace()
	{
		using var ps = psFactory.Create();
		ps.SetCurrentWorkingDirectory(Path.Join(Directory.GetCurrentDirectory(), gitOptions.WorkingDirectory));
		var gitTopLevel = (await ps.InvokeCliAsync("git", "rev-parse", "--show-toplevel")).Results.Single().ToString();
		if (ps.InvocationStateInfo.State != PSInvocationState.Completed)
		{
			throw new InvalidOperationException("Unable to get the git repository root.");
		}

		var rs = psFactory.CreateRunspace();
		rs.SetCurrentWorkingDirectory(gitTopLevel);
		return rs;
	}

	internal async Task<PowerShell> CreateGitToolsPowershell()
	{
		return psFactory.Create(await runspaceFactory.Value);
	}

	private void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				((IDisposable?)runspace)?.Dispose();
				// TODO: dispose more
			}

			runspace = null;
			disposedValue = true;
		}
	}

	public void Dispose()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	internal async Task<PowerShellInvocationResult> InvokeGitToolsAsync(string relativeScriptName, Action<PowerShell> addParameters)
	{
		using var ps = await CreateGitToolsPowershell();
		var scriptPath = Path.Join(gitOptions.GitToolsDirectory, relativeScriptName);

		return await ps.InvokeExternalScriptAsync(scriptPath, addParameters);
	}

}
