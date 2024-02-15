using Microsoft.AspNetCore.DataProtection;
#if IncludeAWS
using Amazon.SimpleSystemsManagement;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
#endif
#if IncludeAzure
using Azure.Identity;
#endif

namespace PrincipleStudios.ScaledGitApp.Api.Environment;

public static class ServiceRegistration
{
	internal static void RegisterEnvironment(this IServiceCollection services,
		bool isProduction,
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
}
