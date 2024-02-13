using System.Management.Automation.Runspaces;
using System.Management.Automation;

namespace PrincipleStudios.ScaledGitApp.ShellUtililties;

public class PowerShellFactory
{
	private readonly InitialSessionState initialSessionState;

	public PowerShellFactory()
	{
		initialSessionState = PowerShellSessionStateUtility.BuildStandardSessionState();
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

	public IPowerShell Create(Runspace? runspace = null)
	{
		return new PowerShellWrapperImplementation(runspace != null
			? PowerShell.Create(runspace)
			: PowerShell.Create(initialSessionState));
	}

}
