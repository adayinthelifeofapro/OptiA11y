using System;
using EPiServer.Shell.Modules;
using EPiServer.Shell.ViewComposition;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace EPiServer.Shell.Web.Mvc;

/// <summary>
///       Constrains the valid routes to the controllers defined in a specific module
///       </summary>
public class ControllerNameRouteConstraint : IRouteConstraint, IParameterPolicy
{
	private readonly ShellModule _module;

	private readonly IViewManager _viewManager;

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Web.Mvc.ControllerNameRouteConstraint" /> class.
	///       </summary>
	public ControllerNameRouteConstraint(ShellModule module, IViewManager viewManager)
	{
		_module = module;
		_viewManager = viewManager;
	}

	/// <summary>
	///       Determines whether the route values contains a controller parameter matching a controller in the module.
	///       </summary>
	/// <param name="httpContext">An object that encapsulates information about the HTTP request.</param>
	/// <param name="route">The object that this constraint belongs to.</param>
	/// <param name="routeKey">The name of the parameter that is being checked.</param>
	/// <param name="values">An object that contains the parameters for the URL.</param>
	/// <param name="routeDirection">An object that indicates whether the constraint check is being performed when an incoming request is being handled or when a URL is being generated.</param>
	/// <returns>
	///       true if a matiching controller is found; otherwise, false.
	///       </returns>
	public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
	{
		string text = values["controller"] as string;
		if (string.IsNullOrEmpty(text))
		{
			return false;
		}
		if (_module.GetControllerType(text) != null || text.Equals("DefaultShellModule", StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		return _viewManager.GetView(_module, text) != null;
	}
}
