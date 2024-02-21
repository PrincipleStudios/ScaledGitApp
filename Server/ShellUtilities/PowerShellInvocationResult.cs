using System.Management.Automation;

namespace PrincipleStudios.ScaledGitApp.ShellUtilities;

public record PowerShellInvocationResult(
	IReadOnlyList<PSObject> Results,
	PSInvocationState InvocationState,
	Exception? InvocationStateException,
	bool HadErrors,
	IReadOnlyList<PSObject>? ErrorContents,
	PowerShellInvocationStreams Streams);

public static class PowerShellInvocationResultExtensions
{

	public static async Task<PowerShellInvocationResult> ToInvocationResult(this Task<PSDataCollection<PSObject>> resultTask, PowerShell ps)
	{
		var results = (await resultTask).ToArray();

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
			Streams: new PowerShellInvocationStreams(
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
