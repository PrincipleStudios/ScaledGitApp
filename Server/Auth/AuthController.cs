using Microsoft.AspNetCore.Mvc;

namespace PrincipleStudios.ScaledGitApp.Auth;

public class AuthController : ControllerBase
{
	[HttpGet]
	[global::Microsoft.AspNetCore.Mvc.Route("/challenge/{scheme}")]
	public IActionResult ChallengeSpecificScheme(string scheme)
	{
		return this.Challenge(scheme);
	}
}
