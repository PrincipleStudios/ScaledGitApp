using PrincipleStudios.ScaledGitApp.ShellUtilities;
using System.Text.RegularExpressions;

namespace PrincipleStudios.ScaledGitApp.Git;

public sealed partial class GitToolsPowerShell : IGitToolsPowerShell
{
	public async Task GitClone(string repository)
	{
		using var pwsh = await CreateGitToolsPowershell();

		// TODO: git tools do not support bare repos, otherwise this should be a bare repo
		ThrowIfHadErrors(await pwsh.InvokeCliAsync("git", "clone", repository, ".", "--quiet", "--no-checkout"));

		// Because this is not a bare repo, we need to go to a fake branch to basically simulate a bare repo.
		// This only affects performance.
		var currentBranch = ToResultStrings(await pwsh.InvokeCliAsync("git", "rev-parse", "--abbrev-ref", "HEAD")).Single();
		ThrowIfHadErrors(await pwsh.InvokeCliAsync("git", "checkout", "--orphan", "__fake", "--quiet"));
		ThrowIfHadErrors(await pwsh.InvokeCliAsync("git", "rm", "-rf", "."));
		ThrowIfHadErrors(await pwsh.InvokeCliAsync("git", "branch", "-D", currentBranch));
	}

	public async Task GitFetch()
	{
		using var pwsh = await CreateGitToolsPowershell();
		ThrowIfHadErrors(await pwsh.InvokeCliAsync("git", "fetch", "--porcelain"));
	}

	private static readonly Regex gitRemoteLine = new Regex(@"^(?<alias>[^\t]+)\t(?<url>[^ ]+) \(fetch\)$");
	public async Task<GitRemoteResult> GitRemote()
	{
		using var pwsh = await CreateGitToolsPowershell();
		var result = ToResultStrings(await pwsh.InvokeCliAsync("git", "remote", "-v"));

		return new GitRemoteResult((
			from line in result
			let match = gitRemoteLine.Match(line)
			where match.Success
			select new GitRemote(match.Groups["alias"].Value, match.Groups["url"].Value)
		).ToArray());
	}

	private static IEnumerable<string> ToResultStrings(PowerShellInvocationResult pwshResult)
	{
		ThrowIfHadErrors(pwshResult);
		return pwshResult.Results.Select(i => i.ToString());
	}

	private static void ThrowIfHadErrors(PowerShellInvocationResult pwshResult)
	{
		if (pwshResult.HadErrors)
			throw GitException.From(pwshResult);
	}
}