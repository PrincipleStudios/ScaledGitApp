namespace PrincipleStudios.ScaledGitApp.Auth;

public class AuthOptions
{
	public required Dictionary<string, IConfigurationSection> Authentication { get; init; }
		= new Dictionary<string, IConfigurationSection>(StringComparer.InvariantCultureIgnoreCase);
}
