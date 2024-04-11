#pragma warning disable CA1861 // Avoid constant arrays as arguments - the envfile list belongs here, and static arrays cannot be created in Program.cs
using dotenv.net;
using PrincipleStudios.ScaledGitApp.Auth;
using PrincipleStudios.ScaledGitApp.BranchingStrategy;
using PrincipleStudios.ScaledGitApp.Commands;
using PrincipleStudios.ScaledGitApp.Environment;
using PrincipleStudios.ScaledGitApp.Git;
using PrincipleStudios.ScaledGitApp.Locales;
using PrincipleStudios.ScaledGitApp.Realtime;
using PrincipleStudios.ScaledGitApp.ShellUtilities;

DotEnv.Load(new DotEnvOptions(envFilePaths: new[] {
	"../.env",
	".env",
}));

var builder = WebApplication.CreateBuilder(args);

#if IncludeAWS
builder.Configuration.AddSecretsManager();
#endif

var services = builder.Services;

services.RegisterAuth(builder.Configuration.GetSection("Auth"));
services.RegisterBranchingStrategy(builder.Configuration.GetSection("Strategy"));
services.RegisterCommands(builder.Configuration.GetSection("Command"));
services.RegisterEnvironment(
	isProduction: builder.Environment.IsProduction(),
	environmentName: builder.Environment.EnvironmentName,
	buildConfig: builder.Configuration.GetSection("build"),
	dataProtectionConfig: builder.Configuration.GetSection("DataProtection")
);
services.RegisterGit(builder.Configuration.GetSection("git"));
services.RegisterLocales(builder.Configuration.GetSection("localization"));
services.RegisterRealtimeNotifications(
	includeAzureSignalR: builder.Configuration["Azure:SignalR:ConnectionString"] != null
);
services.RegisterShellUtilities();

var app = builder.Build();

app.UseHealthChecks("/health");

app.UseForwardedHeaders();
app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

#pragma warning disable ASP0014 // Suggest using top level route registrations - this seems to be necessary to prevent the SPA middleware from overwriting controller requests
app.UseEndpoints(endpoints =>
	{
		endpoints.MapControllers();
		endpoints.MapHub<FullHub>("/hub");
	});
#pragma warning restore ASP0014 // Suggest using top level route registrations


// Keep stray POSTs from hitting the SPA middleware
// Based on a comment in https://github.com/dotnet/aspnetcore/issues/5192
app.MapWhen(context => context.Request.Method == "GET" || context.Request.Method == "CONNECT", (when) =>
{
	// Force lookup so bundle.js doesn't get cached and changes get ignored
	app.UseCacheControlForSpaPages();
	app.UseSpaStaticFiles();
	app.UseSpa(spa =>
	{
#if DEBUG
		if (app.Environment.IsDevelopment())
		{
			spa.Options.SourcePath = "../ui";

			spa.UseViteDevelopmentServer("node_modules/.bin/vite", "--port {port}");
		}
#endif
	});
});

app.Run();
