using System.Management.Automation;

namespace PrincipleStudios.ScaledGitApp.ShellUtilities;

public record PowerShellInvocationStreams(
	IReadOnlyList<DebugRecord> Debug,
	IReadOnlyList<VerboseRecord> Verbose,
	IReadOnlyList<InformationRecord> Information,
	IReadOnlyList<ProgressRecord> Progress,
	IReadOnlyList<WarningRecord> Warning,
	IReadOnlyList<ErrorRecord> Error
);
