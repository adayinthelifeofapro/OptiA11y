using System;
using System.Runtime.CompilerServices;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Modules;
using EPiServer.Shell.ViewComposition;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace EPiServer.Shell.Web.Mvc;

/// <summary>
///       Constrains the valid routes to the controllers defined in a specific module
///       </summary>
public class ModuleAreaConstraint : IRouteConstraint, IParameterPolicy
{
	private readonly ShellModule _module;

	[CompilerGenerated]
	private Injected<IViewManager> _003CViewManager_003Ek__BackingField;

	/// <summary>
	///       The view manager service
	///       </summary>
	public Injected<IViewManager> ViewManager
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _003CViewManager_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_003CViewManager_003Ek__BackingField = value;
		}
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Web.Mvc.ControllerNameRouteConstraint" /> class.
	///       </summary>
	/// <param name="module">The module searched for controllers.</param>
	public ModuleAreaConstraint(ShellModule module)
	{
		_module = module;
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
		if (routeDirection == RouteDirection.IncomingRequest)
		{
			return true;
		}
		if (routeKey.Equals("module", StringComparison.OrdinalIgnoreCase))
		{
			if (values["module"] is ShellModule shellModule)
			{
				return string.Equals(shellModule.Name, _module.Name, StringComparison.OrdinalIgnoreCase);
			}
			return true;
		}
		if (routeKey.Equals("moduleArea", StringComparison.OrdinalIgnoreCase))
		{
			string text = values["moduleArea"] as string;
			if (!string.IsNullOrWhiteSpace(text))
			{
				return string.Equals(text, _module.Name, StringComparison.OrdinalIgnoreCase);
			}
			return true;
		}
		return false;
	}
}
