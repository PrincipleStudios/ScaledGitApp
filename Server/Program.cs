var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
services.AddHealthChecks();
services.AddControllers();

services.Configure<PrincipleStudios.ScaledGitApp.Api.Environment.BuildOptions>(builder.Configuration.GetSection("build"));

services.AddSpaStaticFiles(configuration =>
{
#if DEBUG
	configuration.RootPath = "../ui/dist";
#else
	configuration.RootPath = "wwwroot";
#endif
});

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
