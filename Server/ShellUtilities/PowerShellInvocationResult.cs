using System.Management.Automation;

namespace PrincipleStudios.ScaledGitApp.ShellUtilities;

public record PowerShellInvocationResult(
	IReadOnlyList<PSObject> Results,
	PSInvocationState InvocationState,
	Exception? InvocationStateException,
	int LastExitCode,
	object? LastError,
	PowerShellInvocationStreams Streams)
{
	public bool HadErrors => LastExitCode != 0 || LastError != null;
}

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

		var lastExecutionDetails = await GetLastExecutionDetails(ps);
		var lastExitCode = Convert.ToInt32(lastExecutionDetails["LastExitCode"] ?? 0);

		return new(
			Results: results,
			InvocationState: ps.InvocationStateInfo.State,
			InvocationStateException: ps.InvocationStateInfo.Reason,
			LastExitCode: lastExitCode,
			LastError: lastExecutionDetails["Error"],
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

	private static async Task<System.Collections.Hashtable> GetLastExecutionDetails(PowerShell ps)
	{
		ps.Commands.Clear();
		ps.AddScript("@{ LastExitCode = $global:LastExitCode; Error = $global:error[0] }");
		var outputs = await ps.InvokeAsync();
		return (System.Collections.Hashtable)outputs.Single().BaseObject;
	}
}
