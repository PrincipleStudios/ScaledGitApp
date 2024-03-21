using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace PrincipleStudios.ScaledGitApp.Auth;

public class AuthController : ControllerBase
{
	[HttpGet]
	[Route("/challenge/{scheme}")]
	public IActionResult ChallengeSpecificScheme(string scheme, [FromQuery] string returnUrl)
	{
		return Challenge(new AuthenticationProperties
		{
			RedirectUri = Url.IsLocalUrl(returnUrl) ? returnUrl : "/",
		}, scheme);
	}
}
