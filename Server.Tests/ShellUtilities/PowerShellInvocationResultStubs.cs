﻿using OpenTelemetry.Trace;
using PrincipleStudios.ScaledGitApp.ShellUtilities;
using System.Management.Automation;

namespace PrincipleStudios.ScaledGitApp.ShellUtilities;

public static class PowerShellInvocationResultStubs
{
	public static readonly PowerShellInvocationResult Empty = new(
		Results: Array.Empty<PSObject>(),
		InvocationState: PSInvocationState.Completed,
		InvocationStateException: null,
		HadErrors: false,
		ErrorContents: Array.Empty<PSObject>(),
		Streams: new(
			Debug: Array.Empty<DebugRecord>(),
			Verbose: Array.Empty<VerboseRecord>(),
			Information: Array.Empty<InformationRecord>(),
			Progress: Array.Empty<ProgressRecord>(),
			Warning: Array.Empty<WarningRecord>(),
			Error: Array.Empty<ErrorRecord>()
		)
	);

	public static PowerShellInvocationResult WithResults(params string[] lines) =>
		Empty with
		{
			Results = lines.Select(line => new PSObject(line)).ToArray()
		};

	public static PowerShellInvocationResult WithCliErrors(params string[] lines) =>
		Empty with
		{
			HadErrors = true,
			Streams = Empty.Streams with
			{
				Error = (
					from line in lines
					select new ErrorRecord(new RemoteException(line), "NativeCommandError", ErrorCategory.NotSpecified, line)
				).ToArray()
			}
		};
}
