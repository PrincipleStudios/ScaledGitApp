namespace PrincipleStudios.ScaledGitApp.Commands;

public class CommandCacheOptions
{
	public bool DefaultEnabled { get; set; } = true;
	public IConfigurationSection? TypeSettings { get; init; }
}
