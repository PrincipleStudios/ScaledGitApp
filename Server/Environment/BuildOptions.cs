namespace PrincipleStudios.ScaledGitApp.Environment;

public record BuildOptions
{
	public string GitHash { get; init; } = "HEAD";
	public string Tag { get; init; } = "unknown";
}
