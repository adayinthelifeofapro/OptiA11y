using System;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Modules;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace EPiServer.Shell.Web;

/// <summary>
///       Extension methods for the HttpContext class.
///       </summary>
public static class HttpContextExtensions
{
	/// <summary>
	///       Gets the path to a controller action
	///       </summary>
	/// <param name="context">The current http context</param>
	/// <param name="controllerType">The type of controller to link</param>
	/// <param name="action">The action name on the controller</param>
	/// <returns>
	/// </returns>
	public static string GetControllerPath(this HttpContext context, Type controllerType, string action)
	{
		ArgumentNullException.ThrowIfNull(controllerType, "controllerType");
		if (!controllerType.IsSubclassOf(typeof(ControllerBase)))
		{
			return null;
		}
		RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
		routeValueDictionary["action"] = action;
		if (ServiceProviderExtensions.GetInstance<ModuleTable>(ServiceLocator.Current).TryGetModule(controllerType.Assembly, out var shellModule))
		{
			routeValueDictionary["controller"] = shellModule.GetRouteSegmentForController(controllerType.Name);
			string pathByRouteValues = context.RequestServices.GetService<LinkGenerator>().GetPathByRouteValues(context, null, routeValueDictionary);
			pathByRouteValues = pathByRouteValues.TrimStart('/');
			return Paths.ToResource(shellModule.Name, pathByRouteValues);
		}
		routeValueDictionary["controller"] = StringExtensions.TrimControllerSuffix(controllerType.Name);
		return context.RequestServices.GetService<LinkGenerator>().GetPathByRouteValues(context, null, routeValueDictionary);
	}
}
