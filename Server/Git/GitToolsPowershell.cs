using Microsoft.Extensions.Options;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace PrincipleStudios.ScaledGitApp.Git;

public sealed class GitToolsPowershell : IDisposable
{
	private static readonly string[] continuePreferences =
	{
		"DebugPreference",
		"ProgressPreference",
		"VerbosePreference",
		"WarningPreference",
		"InformationPreference",
	};
	private readonly GitOptions gitOptions;
	private readonly Lazy<Task<Runspace>> runspaceFactory;
	private Runspace? runspace;
	private bool disposedValue;

	public GitToolsPowershell(IOptions<GitOptions> options)
	{
		gitOptions = options.Value;
		runspaceFactory = new Lazy<Task<Runspace>>(CreateRunspace);
	}

	async Task<Runspace> CreateRunspace()
	{
		var iss = InitialSessionState.CreateDefault();
		iss.ExecutionPolicy = Microsoft.PowerShell.ExecutionPolicy.Unrestricted;
		iss.Variables.Add(new SessionStateVariableEntry("ErrorActionPreference", ActionPreference.Stop, "Override error action preference"));
		foreach (var pref in continuePreferences)
			iss.Variables.Add(new SessionStateVariableEntry(pref, ActionPreference.Continue, "Override stream preference"));
		using var tempRunspace = RunspaceFactory.CreateRunspace(iss);
		tempRunspace.Open();
		tempRunspace.SessionStateProxy.Path.SetLocation(Path.Join(Directory.GetCurrentDirectory(), gitOptions.WorkingDirectory));

		using var ps = PowerShell.Create(tempRunspace);
		ps.AddScript("git rev-parse --show-toplevel");
		var gitTopLevel = (await ps.InvokeAsync()).Single().ToString();
		if (ps.InvocationStateInfo.State != PSInvocationState.Completed)
		{
			throw new InvalidOperationException("Unable to get the git repository root.");
		}

		var rs = RunspaceFactory.CreateRunspace(iss);
		rs.Open();
		rs.SessionStateProxy.Path.SetLocation(gitTopLevel);
		return rs;
	}

	internal async Task<PowerShell> CreateGitToolsPowershell()
	{
		var ps = PowerShell.Create(await runspaceFactory.Value);
		return ps;
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

	internal async Task<PowershellInvocationResult> InvokeGitToolsAsync(string relativeScriptName, Action<PowerShell> addParameters)
	{
		using var ps = await CreateGitToolsPowershell();
		var scriptPath = Path.Join(gitOptions.GitToolsDirectory, relativeScriptName);

		addParameters(ps.AddCommand(ps.Runspace.SessionStateProxy.InvokeCommand.GetCommand(scriptPath, CommandTypes.ExternalScript)));

		return await PowershellInvocationResult.InvokeAsync(ps);
	}

}

public record PowershellInvocationResult(
	IReadOnlyList<PSObject> Results,
	PSInvocationState InvocationState,
	Exception InvocationStateException,
	bool HadErrors,
	IReadOnlyList<PSObject>? ErrorContents,
	PowershellInvocationStreams Streams)
{

	public static async Task<PowershellInvocationResult> InvokeAsync(PowerShell ps)
	{
		var results = (await ps.InvokeAsync()).ToArray();

		var debugRecords = ps.Streams.Debug.ToArray();
		var verboseRecords = ps.Streams.Verbose.ToArray();
		var informationRecords = ps.Streams.Information.ToArray();
		var progressRecords = ps.Streams.Progress.ToArray();
		var warningRecords = ps.Streams.Warning.ToArray();
		var errorRecords = ps.Streams.Error.ToArray();

		var originalHadErrors = ps.HadErrors;

		return new(
			Results: results,
			InvocationState: ps.InvocationStateInfo.State,
			InvocationStateException: ps.InvocationStateInfo.Reason,
			HadErrors: ps.HadErrors,
			ErrorContents: originalHadErrors ? await GetError(ps) : null,
			Streams: new PowershellInvocationStreams(
				debugRecords,
				verboseRecords,
				informationRecords,
				progressRecords,
				warningRecords,
				errorRecords
			)
		);
	}

	private static async Task<IReadOnlyList<PSObject>> GetError(PowerShell ps)
	{
		ps.Commands.Clear();
		ps.AddScript("$error");
		return (await ps.InvokeAsync()).ToArray();
	}
}

public record PowershellInvocationStreams(
	IReadOnlyList<DebugRecord> Debug,
	IReadOnlyList<VerboseRecord> Verbose,
	IReadOnlyList<InformationRecord> Information,
	IReadOnlyList<ProgressRecord> Progress,
	IReadOnlyList<WarningRecord> Warning,
	IReadOnlyList<ErrorRecord> Error
);
