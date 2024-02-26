using System.Management.Automation.Runspaces;
using System.Management.Automation;

namespace PrincipleStudios.ScaledGitApp.ShellUtilities;

public class PowerShellFactory
{
	private readonly InitialSessionState initialSessionState;

	public PowerShellFactory()
	{
		initialSessionState = PowerShellSessionStateUtility.BuildStandardSessionState();
	}

	public virtual IPowerShell Create(Runspace? runspace = null)
	{
		return new PowerShellWrapperImplementation(runspace != null
			? PowerShell.Create(runspace)
			: PowerShell.Create(initialSessionState));
	}

}
