var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
services.AddHealthChecks();
services.AddControllers();

services.Configure<PrincipleStudios.ScaledGitApp.Api.Environment.BuildOptions>(builder.Configuration.GetSection("build"));

var app = builder.Build();

app.MapHealthChecks("/health");
app.MapControllers();

app.Run();
