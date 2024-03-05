using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrincipleStudios.ScaledGitApp.Git;

public static class GitConventions
{
	const string headsPrefix = "refs/heads/";
	public static string ToFullyQualifiedBranchName(string branchName) => branchName.StartsWith(headsPrefix) ? branchName : $"{headsPrefix}{branchName}";

}
