using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Routing;

namespace EPiServer.Shell.Navigation;

/// <summary>
///       MVC Route MenuItem
///       </summary>
public class RouteMenuItem : MenuItem
{
	/// <summary>
	///       Extra route values
	///       </summary>
	public RouteValueDictionary RouteValues { get; private set; }

	/// <summary>
	///        Initializes a new instance of the <see cref="T:EPiServer.Shell.Navigation.RouteMenuItem" /> class.
	///       </summary>
	/// <param name="text">Link text</param>
	/// <param name="path">Unique path for the menu item.</param>
	/// <param name="routeValues">Extra route values</param>
	public RouteMenuItem(string text, string path, RouteValueDictionary routeValues)
		: base(text, path)
	{
		RouteValues = routeValues ?? new RouteValueDictionary();
	}

	/// <summary>
	///       Determines whether the menu item is selected withing the the specified request context.
	///       </summary>
	/// <param name="requestContext">The request context.</param>
	public override bool IsSelected(HttpContext requestContext)
	{
		RouteValueDictionary routeValues = requestContext.Request.RouteValues;
		Endpoint endpoint = requestContext.GetEndpoint();
		ArgumentNullException.ThrowIfNull(requestContext, "requestContext");
		if (endpoint != null)
		{
			foreach (object item in endpoint.Metadata)
			{
				if (item is PageRouteMetadata)
				{
					return false;
				}
			}
		}
		if (routeValues.Count == 0)
		{
			return false;
		}
		foreach (KeyValuePair<string, object?> routeValue in RouteValues)
		{
			if (routeValues.TryGetValue(routeValue.Key, out object value) && !((string)routeValue.Value).Equals((string)value, StringComparison.OrdinalIgnoreCase))
			{
				if (!routeValue.Key.Equals("controller", StringComparison.OrdinalIgnoreCase) || !routeValues.TryGetValue("routeController", out value))
				{
					return false;
				}
				if (!((string)routeValue.Value).Equals((string)value, StringComparison.OrdinalIgnoreCase))
				{
					return false;
				}
			}
		}
		return true;
	}
}
