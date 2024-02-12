namespace PrincipleStudios.ScaledGitApp.Git;

public record GitOptions
{
	public string? Repository { get; init; }
	public string WorkingDirectory { get; init; } = "./";
	public string? GitToolsDirectory { get; init; }
}
