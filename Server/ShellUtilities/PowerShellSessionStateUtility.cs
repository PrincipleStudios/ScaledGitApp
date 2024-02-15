using System.Management.Automation.Runspaces;
using System.Management.Automation;
using System.Runtime.InteropServices;

namespace PrincipleStudios.ScaledGitApp.ShellUtilities;

public static class PowerShellSessionStateUtility
{
	private static readonly string[] continuePreferences =
	{
		"DebugPreference",
		"ProgressPreference",
		"VerbosePreference",
		"WarningPreference",
		"InformationPreference",
	};

	public static InitialSessionState BuildStandardSessionState()
	{
		var result = InitialSessionState.CreateDefault();

		// Only Windows has an Execution Policy to be set
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			result.ExecutionPolicy = Microsoft.PowerShell.ExecutionPolicy.Unrestricted;

		// Set ErrorAction to always stop; we should be in control of all scripts that are running, so this should not occur.
		// If it does, we want to get the error.
		result.Variables.Add(new SessionStateVariableEntry("ErrorActionPreference", ActionPreference.Stop, "Override error action preference"));

		// Output all streams for access to C# because the user isn't going to see these anyway
		foreach (var pref in continuePreferences)
			result.Variables.Add(new SessionStateVariableEntry(pref, ActionPreference.Continue, "Override stream preference"));

		return result;
	}
}
