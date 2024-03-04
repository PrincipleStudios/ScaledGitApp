namespace PrincipleStudios.ScaledGitApp.Git;

public class GitToolsPowerShellShould
{
	readonly GitToolsPowerShellFixture fixture = new GitToolsPowerShellFixture();

	[Fact]
	public void Have_the_default_Upstream_branch()
	{
		var target = fixture.Create();

		var upstreamBranchName = target.UpstreamBranchName;

		Assert.Equal("refs/remotes/origin/_upstream", upstreamBranchName);
	}

	[InlineData("refs/heads/PS-123", "refs/remotes/origin/PS-123")]
	[InlineData("feature/PS-123", "refs/remotes/origin/feature/PS-123")]
	[InlineData("_upstream", "refs/remotes/origin/_upstream")]
	// no match, but it still results in the wildcard
	[InlineData("refs/pull/100/head", "refs/remotes/origin/refs/pull/100/head")]
	[Theory]
	public void Map_remote_branches_to_local_tracking_name(string nameOnRemote, string? localTrackingName)
	{
		var target = fixture.Create();

		var actual = target.ToLocalTrackingBranchName(nameOnRemote);

		Assert.Equal(localTrackingName, actual);
	}

	[InlineData("refs/pull/100/head", "refs/remotes/github-pr/100")]
	[InlineData("_upstream", "refs/remotes/github/_upstream")]
	[Theory]
	public void Map_remote_branches_with_alternate_mapping(string nameOnRemote, string? localTrackingName)
	{
		var target = fixture.Create(cloneConfiguration: Defaults.DefaultCloneConfiguration with
		{
			FetchMapping = [
				FetchMapping.Parse("+refs/heads/*:refs/remotes/github/*"),
				FetchMapping.Parse("+refs/pull/*/head:refs/remotes/github-pr/*")
			]
		});

		var actual = target.ToLocalTrackingBranchName(nameOnRemote);

		Assert.Equal(localTrackingName, actual);
	}
}
