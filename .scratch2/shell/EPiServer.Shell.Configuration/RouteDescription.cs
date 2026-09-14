using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using EPiServer.Shell.Web;

namespace EPiServer.Shell.Configuration;

/// <summary>
///       Describes a route to use for a module
///       </summary>
[Serializable]
public class RouteDescription
{
	/// <summary>
	///       The URL pattern for the route.
	///       </summary>
	/// <remarks>
	///       Defaults to <code>{moduleArea}/{controller}/{action}/{id}</code> if not set.
	///       </remarks>
	[XmlAttribute("url")]
	public string Url { get; set; }

	/// <summary>
	///       Gets or sets an optional class for routing. The class must inherit 
	///       </summary>
	[XmlAttribute("type")]
	public string Type { get; set; }

	/// <summary>
	///       Gets or sets the values to use if the URL does not contain all the parameters.
	///       </summary>
	[XmlArray("defaults")]
	[XmlArrayItem("add")]
	public List<KeyValueElement> Defaults { get; set; }

	/// <summary>
	///       Gets or sets a collection of key/value pairs defining valid values for
	///       URL parameters.
	///       </summary>
	[XmlArray("constraints")]
	[XmlArrayItem("add")]
	public List<KeyValueElement> Constraints { get; set; }

	/// <summary>
	///       Gets or sets custom values that are passed to the route handler, but which
	///       are not used to determine whether the route matches a URL pattern.
	///       </summary>
	[XmlArray("dataTokens")]
	[XmlArrayItem("add")]
	public List<KeyValueElement> DataTokens { get; set; }

	/// <summary>
	///       A prefix applied to to all controllers routed to this route. This makes it possible to "scope" controllers so they don't conflict with application controllers.
	///       </summary>
	[XmlAttribute("controllerPrefix")]
	public string ControllerPrefix { get; set; }

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Configuration.RouteDescription" /> class.
	///       Will set Url to the default value "{moduleArea}/{controller}/{action}/{id}".
	///       </summary>
	public RouteDescription()
	{
		Url = "{moduleArea}/{controller}/{action}/{id}";
		Defaults = new List<KeyValueElement>();
		Constraints = new List<KeyValueElement>();
		DataTokens = new List<KeyValueElement>();
	}

	/// <summary>
	///       Removes the <see cref="P:EPiServer.Shell.Configuration.RouteDescription.ControllerPrefix" /> from the beginning of the controller name and "Controller" from the end.
	///       </summary>
	/// <param name="controllerName">The controller type name to trim.</param>
	/// <returns>A leaner controller name.</returns>
	public string TrimControllerName(string controllerName)
	{
		controllerName = StringExtensions.TrimControllerSuffix(controllerName);
		if (string.IsNullOrEmpty(controllerName) || string.IsNullOrEmpty(ControllerPrefix) || !controllerName.StartsWith(ControllerPrefix, StringComparison.OrdinalIgnoreCase))
		{
			return controllerName;
		}
		return controllerName.Substring(ControllerPrefix.Length);
	}
}
