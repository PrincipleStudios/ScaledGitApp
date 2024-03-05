namespace PrincipleStudios.ScaledGitApp.Git;

public static class Defaults
{
	public static readonly GitCloneConfiguration DefaultCloneConfiguration = new GitCloneConfiguration(
		GitRootDirectory: "./",
		"origin",
		"_upstream",
		[FetchMapping.Parse("+refs/heads/*:refs/remotes/origin/*")]
	);
}
