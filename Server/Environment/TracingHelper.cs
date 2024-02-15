using System.Diagnostics;

namespace PrincipleStudios.ScaledGitApp.Environment;

public static class TracingHelper
{
	public static readonly ActivitySource ActivitySource = new($"{nameof(PrincipleStudios)}.{nameof(ScaledGitApp)}");

	public static Activity? StartActivity(string name, ActivityKind kind = ActivityKind.Internal)
	{
		return ActivitySource.StartActivity(name, kind);
	}
}