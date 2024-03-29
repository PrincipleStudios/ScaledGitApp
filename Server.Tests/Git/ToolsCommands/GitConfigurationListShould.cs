using Moq;
using PrincipleStudios.ScaledGitApp.ShellUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

public class GitConfigurationListShould
{
	private readonly PowerShellFixture fixture = new();
	private readonly string[] standardConfiguration = [
		"remote.azure.fetch=+refs/heads/*:refs/remotes/azure/*",
		"remote.origin.fetch=+refs/heads/*:refs/remotes/origin/*",
		"remote.origin.fetch=+refs/pull/*/head:refs/remotes/origin-pr/*",
		"scaled-git.upstreambranch=my-upstream"
	];

	[InlineData("remote.azure.fetch", "+refs/heads/*:refs/remotes/azure/*")]
	// Multiple values
	[InlineData("remote.origin.fetch", "+refs/heads/*:refs/remotes/origin/*")]
	[InlineData("remote.origin.fetch", "+refs/pull/*/head:refs/remotes/origin-pr/*")]
	// case insensitive
	[InlineData("scaled-git.upstreamBranch", "my-upstream")]
	[Theory]
	public async Task FindStandardConfigurationFields(string key, string expectedValue)
	{
		var verifiable = fixture.MockPowerShell.Verifiable(
			ps => ps.PowerShellInvoker.InvokeCliAsync("git", "config", "--list"),
			s => s.ReturnsAsync(
				PowerShellInvocationResultStubs.WithResults(standardConfiguration)
			)
		);

		var target = new GitConfigurationList();

		var actualConfiguration = await target.Execute(fixture.Create());

		var actualValues = Assert.Contains(key, actualConfiguration);
		Assert.Contains(expectedValue, actualValues);
		verifiable.Verify(Times.Once);
	}

}
