namespace PrincipleStudios.ScaledGitApp.Environment;

public static class CacheControlExtensions
{
	private static readonly string[] varyByHeader = ["Accept-Encoding"];

	public static void UseCacheControlForSpaPages(this IApplicationBuilder app)
	{
		app.Use(async (context, next) =>
		{
			context.Response.GetTypedHeaders().CacheControl =
				new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
				{
					MustRevalidate = true,
					Public = true,
				};
			context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] = varyByHeader;

			await next().ConfigureAwait(false);
		});
	}
}
