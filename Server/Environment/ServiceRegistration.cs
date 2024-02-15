#if IncludeAWS
using Amazon.SimpleSystemsManagement;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
#endif
#if IncludeAzure
using Azure.Identity;
#endif
using Microsoft.AspNetCore.DataProtection;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;

namespace PrincipleStudios.ScaledGitApp.Environment;

public static class ServiceRegistration
{
	internal static void RegisterEnvironment(this IServiceCollection services,
		bool isProduction,
		string environmentName,
		IConfigurationSection buildConfig,
		IConfigurationSection dataProtectionConfig)
	{
		services.AddHealthChecks();
		services.AddControllers();

		services.Configure<BuildOptions>(buildConfig);

		services.AddSpaStaticFiles(configuration =>
		{
			configuration.RootPath = "wwwroot";
		});

		services.RegisterDataProtection(isProduction, dataProtectionConfig);
		services.RegisterOpenTelemetry(environmentName, buildConfig);
	}

	private static void RegisterDataProtection(this IServiceCollection services, bool isProduction, IConfigurationSection dataProtectionConfig)
	{
		if (isProduction)
		{
			var dataProtectionBuilder = services.AddDataProtection();
#if IncludeAzure
			if (dataProtectionConfig["AzureKeyVault"] is string keyUri &&
				dataProtectionConfig["AzureBlobStorage"] is string blobUri)
			{
				dataProtectionBuilder
					.PersistKeysToAzureBlobStorage(new Uri(blobUri), new DefaultAzureCredential())
					.ProtectKeysWithAzureKeyVault(new Uri(keyUri), new DefaultAzureCredential());
			}
#endif
#if IncludeAWS
			if (dataProtectionConfig["Aws_SSM_Path"] is string appSsmPath)
			{
				services.AddAWSService<IAmazonSimpleSystemsManagement>(new AWSOptions { Credentials = new ECSTaskCredentials() });
				services.AddDataProtection().PersistKeysToAWSSystemsManager($"{appSsmPath}/DATA_PROTECTION");
			}
#endif
		}
	}

	private static void RegisterOpenTelemetry(this IServiceCollection services, string environmentName, IConfigurationSection buildConfig)
	{
		services
			.AddOpenTelemetry()
			.WithTracing(tracerProviderBuilder =>
			{
				tracerProviderBuilder.AddOtlpExporter();
				tracerProviderBuilder
					.AddSource(TracingHelper.ActivitySource.Name)
					.ConfigureResource(resource =>
						resource.AddService(TracingHelper.ActivitySource.Name, serviceVersion: buildConfig["GitHash"] ?? "local")
						.AddAttributes(new Dictionary<string, object>
						{
							{ "deployment.environment", environmentName }
						}))
					.AddAspNetCoreInstrumentation(cfg =>
					{
						cfg.RecordException = true;
					});
			});
	}
}
