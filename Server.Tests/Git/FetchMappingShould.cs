using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrincipleStudios.ScaledGitApp.Git;

public class FetchMappingShould
{
	[InlineData("+refs/heads/*:refs/remotes/origin/*", "refs/heads/temp", "refs/remotes/origin/temp")]
	[InlineData("+refs/heads/*:refs/remotes/origin/*", "refs/heads/feature/PS-123", "refs/remotes/origin/feature/PS-123")]
	[InlineData("+refs/heads/*:refs/remotes/origin/*", "refs/pull/100/head", null)]
	[InlineData("+refs/pull/*/head:refs/remotes/origin-pr/*", "refs/pull/100/head", "refs/remotes/origin-pr/100")]
	[InlineData("+refs/pull/100/head:refs/remotes/pr-100", "refs/pull/100/head", "refs/remotes/pr-100")]
	[InlineData("+refs/pull/100/head:refs/remotes/pr-100", "refs/pull/101/head", null)]
	[Theory]
	public void Given_refspec_and_input_return_expected_result(string refspec, string input, string? expected)
	{
		var fetchMapping = FetchMapping.Parse(refspec);
		var matched = fetchMapping.TryApply(input, out var actual);

		if (expected == null)
		{
			Assert.False(matched);
			Assert.Null(actual);
		}
		else
		{
			Assert.True(matched);
			Assert.Equal(expected, actual);
		}
	}
}
