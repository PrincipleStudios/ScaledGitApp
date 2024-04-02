
using Microsoft.AspNetCore.Authentication;

namespace PrincipleStudios.ScaledGitApp.Api.Auth;

public class LoginSchemeController(IAuthenticationSchemeProvider authenticationSchemeProvider) : ApiLoginSchemesControllerBase
{
	protected override async Task<GetLoginSchemesActionResult> GetLoginSchemes()
	{
		return GetLoginSchemesActionResult.Ok(
			from scheme in await authenticationSchemeProvider.GetAllSchemesAsync()
			where !string.IsNullOrEmpty(scheme.DisplayName)
			select scheme.DisplayName ?? throw new InvalidOperationException()
		);
	}
}
