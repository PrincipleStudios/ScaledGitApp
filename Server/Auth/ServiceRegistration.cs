﻿using Microsoft.AspNetCore.Authentication.Cookies;

namespace PrincipleStudios.ScaledGitApp.Auth;

public static class ServiceRegistration
{
	internal static void RegisterAuth(this IServiceCollection services, IConfigurationSection configurationSection)
	{
		/* Authentication and Authorization in ASP.NET is built into a series of
		 * separated concerns.
		 * 
		 * Authentication is built with a series of schemes, each of which may have
		 * implemented up to three types of scheme implementations.
		 * - "Authenticate Schemes" load information about the user from the
		 *   HttpContext to create the ClaimsPrincipal.
		 * - "Sign In Schemes" save information about the user (such as to the
		 *   HttpResponse).
		 * - "Challenge Schemes" are used to request information from the user to
		 *   authenticate themselves, such as redirecting to an OAuth page.
		 *   
		 * Each scheme must implement at least one of the above types. They get
		 * composed to make a complete solution.
		 * - .NET's default cookie scheme only supports Authenticate and Sign In; it
		 *   reads or writes cookies. There is no way to "challenge" a user to sign in
		 *   via a Cookie scheme.
		 * - .NET's OAuth schemes only support Challenge; they save the received user
		 *   claims via the configured Sign In scheme.
		 * - Something like HTTP Basic Authentication or an API Key rule would be
		 *   implemented via only an Authenticate scheme; the server does not send
		 *   credentials to the end user for these rules.
		 * 
		 * Authorization uses "policies" to authorize requests. These load credentials
		 * from one or more "Authenticate Schemes", then applies a set of
		 * requirements to the loaded ClaimsPrincipal. If the claims do not pass the
		 * requirements, either the `OnRedirectToAccessDenied` or `OnRedirectToLogin`
		 * is called, depending on whether the Authentication Scheme was able to load
		 * any credentials.
		 * */

		services.AddAuthorization(options =>
		{
			options.AddPolicy("AuthenticatedUser", builder =>
			{
				// Standard authentication loads authentication from cookies
				builder.AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme);

				// We want config-based rules, but every policy must have a requirement.
				// This "always allowed" requirement satisfies that rule.
				builder.RequireAssertion(context => true);
			});
		});


		services
			.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
			})
			.AddCookie(options =>
			{
				options.Cookie.Expiration = null;
				options.Cookie.Path = "/";
				options.Events.OnRedirectToAccessDenied =
					options.Events.OnRedirectToLogin = c =>
					{
						c.Response.StatusCode = 401;
						return Task.CompletedTask;
					};
			});
	}
}
