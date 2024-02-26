
using Microsoft.Extensions.Options;
using PrincipleStudios.ScaledGitApp.Environment;

namespace PrincipleStudios.ScaledGitApp.Api.Environment
{
	public class EnvironmentController : ApiEnvControllerBase
	{
		private readonly BuildOptions buildOptions;

		public EnvironmentController(IOptions<BuildOptions> options)
		{
			buildOptions = options.Value;
		}

		protected override Task<GetInfoActionResult> GetInfo()
		{
			return Task.FromResult(
				GetInfoActionResult.Ok(new(buildOptions.GitHash, buildOptions.Tag))
			);
		}
	}
}
