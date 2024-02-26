using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace PrincipleStudios.ScaledGitApp.Environment;

public class MvcActionTracing : IAsyncActionFilter
{
	public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
	{
		var activityName = context.ActionDescriptor switch
		{
			ControllerActionDescriptor { ControllerName: var controllerName, ActionName: var actionName } => $"{controllerName}.{actionName}",
			_ => null,
		};
		if (activityName == null)
		{
			await next();
		}
		else if (Activity.Current is Activity originalActivity)
		{
			originalActivity.DisplayName = activityName;
			await next();
		}
		else
		{
			using var activity = TracingHelper.StartActivity(activityName);
			await next();
		}
	}
}