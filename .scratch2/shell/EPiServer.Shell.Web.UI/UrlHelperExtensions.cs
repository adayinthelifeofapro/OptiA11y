using System;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace EPiServer.Shell.Web.UI;

/// <summary>
///       Extension methods for the <see cref="T:Microsoft.AspNetCore.Mvc.Routing.UrlHelper" /> class.
///       </summary>
public static class UrlHelperExtensions
{
	/// <summary>
	///       Generates a fully qualified URL for an action method by using the specified action name, controller name, route values and base URL.
	///       </summary>
	/// <param name="urlHelper">The URL helper.</param>
	/// <param name="request">
	/// </param>
	/// <param name="actionName">The name of the action method.</param>
	/// <param name="controllerName">Name of the controller.</param>
	/// <param name="routeValues">An object that contains the parameters for a route.</param>
	/// <param name="baseUri">The base URI.</param>
	/// <returns>The fully qualified URL to an action method.</returns>
	public static string Action(this UrlHelper urlHelper, HttpRequest request, string actionName, string controllerName, RouteValueDictionary routeValues, Uri baseUri)
	{
		string text = urlHelper.Action(actionName, controllerName, routeValues);
		if (text != null)
		{
			if (baseUri == null)
			{
				baseUri = new Uri(string.Format("{0}://{1}", request.IsHttps ? "https" : "http", request.Host));
			}
			string text2 = (baseUri.IsDefaultPort ? string.Empty : (":" + Convert.ToString(baseUri.Port, CultureInfo.InvariantCulture)));
			text = baseUri.Scheme + Uri.SchemeDelimiter + baseUri.Host + text2 + text;
		}
		return text;
	}
}
