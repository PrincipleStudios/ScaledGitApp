using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace PrincipleStudios.ScaledGitApp.Git;

public class FetchMapping(bool allowNonFastForward, Regex remoteMapping, IReadOnlyList<string> localMappingParts)
{
	public bool AllowNonFastForward => allowNonFastForward;
	public Regex RemoteMapping => remoteMapping;
	public IReadOnlyList<string> LocalMappingParts => localMappingParts;

	public static FetchMapping Parse(string refspec)
	{
		var allowNonFastForward = refspec.StartsWith('+');
		var parts = refspec.Split(':');
		if (parts.Length > 2) throw new ArgumentException("Invalid refspec", nameof(refspec));
		var remoteMappingPattern = parts[0];
		var remoteParts = remoteMappingPattern.Split('*');
		if (remoteParts.Length > 2) throw new ArgumentException("Invalid refspec", nameof(refspec));
		if (allowNonFastForward) remoteParts[0] = remoteParts[0].Substring(1); // remove the '+'

		var localMappingPattern = parts[1];
		var localParts = localMappingPattern.Split('*');
		if (localParts.Length != remoteParts.Length) throw new ArgumentException("Invalid refspec", nameof(refspec));

		return remoteParts.Length == 1
			? new FetchMapping(allowNonFastForward, new Regex(Regex.Escape(remoteParts[0])), localParts)
			: new FetchMapping(
				allowNonFastForward,
				new Regex($"^{Regex.Escape(remoteParts[0])}(.+){Regex.Escape(remoteParts[1])}$"),
				localParts
			);
	}

	public bool TryApply(string input, [NotNullWhen(true)] out string? output)
	{
		var match = remoteMapping.Match(input);
		if (!match.Success)
		{
			output = null;
			return false;
		}
		if (match.Groups.Count == 1)
			output = localMappingParts[0];
		else
			output = $"{localMappingParts[0]}{match.Groups[1].Value}{localMappingParts[1]}";
		return true;
	}
}
