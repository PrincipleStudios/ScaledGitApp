namespace PrincipleStudios.ScaledGitApp.Git;

public record GitCloneConfiguration(string GitRootDirectory, string RemoteName, string UpstreamBranchName, IReadOnlyList<FetchMapping> FetchMapping);
