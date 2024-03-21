using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace PrincipleStudios.ScaledGitApp.Auth;

public class AuthController : ControllerBase
{
	[HttpGet]
	[Route("/challenge/{scheme}")]
	public async Task<IActionResult> ChallengeSpecificScheme(string scheme, [FromQuery] string returnUrl)
	{
		await HttpContext.SignOutAsync();
		return Challenge(new AuthenticationProperties
		{
			RedirectUri = Url.IsLocalUrl(returnUrl) ? returnUrl : "/",
		}, scheme);
	}
}
