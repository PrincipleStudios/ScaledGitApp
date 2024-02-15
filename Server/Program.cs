#pragma warning disable CA1861 // Avoid constant arrays as arguments - the envfile list belongs here, and static arrays cannot be created in Program.cs
using dotenv.net;
using PrincipleStudios.ScaledGitApp.Api.Environment;
using PrincipleStudios.ScaledGitApp.Git;
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

services.RegisterEnvironment(
	isProduction: builder.Environment.IsProduction(),
	buildConfig: builder.Configuration.GetSection("build"),
	dataProtectionConfig: builder.Configuration.GetSection("DataProtection")
);
services.RegisterGit(builder.Configuration.GetSection("git"));
services.RegisterShellUtilities();

var app = builder.Build();

app.UseHealthChecks("/health");

app.UseRouting();

#pragma warning disable ASP0014 // Suggest using top level route registrations - this seems to be necessary to prevent the SPA middleware from overwriting controller requests
app.UseEndpoints(endpoints =>
	{
		endpoints.MapControllers();
	});
#pragma warning restore ASP0014 // Suggest using top level route registrations


// Keep stray POSTs from hitting the SPA middleware
// Based on a comment in https://github.com/dotnet/aspnetcore/issues/5192
app.MapWhen(context => context.Request.Method == "GET" || context.Request.Method == "CONNECT", (when) =>
{
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
