namespace PrincipleStudios.ScaledGitApp.Auth;

public class AuthOptions
{
	public Dictionary<string, IConfigurationSection> Authentication { get; init; }
		= new Dictionary<string, IConfigurationSection>(StringComparer.InvariantCultureIgnoreCase);

	public IReadOnlyList<string> AllowedUsers { get; init; } = Array.Empty<string>();
}
