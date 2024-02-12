using System.Management.Automation.Runspaces;
using System.Management.Automation;

namespace PrincipleStudios.ScaledGitApp.ShellUtililties
{
	public class PowerShellFactory
	{
		private static readonly string[] continuePreferences =
		{
			"DebugPreference",
			"ProgressPreference",
			"VerbosePreference",
			"WarningPreference",
			"InformationPreference",
		};
		private readonly InitialSessionState initialSessionState;

		public PowerShellFactory()
		{
			initialSessionState = InitialSessionState.CreateDefault();
			initialSessionState.ExecutionPolicy = Microsoft.PowerShell.ExecutionPolicy.Unrestricted;
			initialSessionState.Variables.Add(new SessionStateVariableEntry("ErrorActionPreference", ActionPreference.Stop, "Override error action preference"));
			foreach (var pref in continuePreferences)
				initialSessionState.Variables.Add(new SessionStateVariableEntry(pref, ActionPreference.Continue, "Override stream preference"));
		}

		public Runspace CreateRunspace(Action<InitialSessionState>? configure = null)
		{
			var iss = initialSessionState;

			if (configure != null)
			{
				iss = initialSessionState.Clone();
				configure.Invoke(iss);
			}

			var result = RunspaceFactory.CreateRunspace(iss);
			result.Open();
			return result;
		}

		public PowerShell Create(Runspace? runspace = null)
		{
			return runspace != null
				? PowerShell.Create(runspace)
				: PowerShell.Create(initialSessionState);
		}

	}
}
