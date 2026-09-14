using System.Collections.Generic;

namespace EPiServer.Shell.Modules;

/// <summary>
///       Used to pass configurable information about a route.
///       </summary>
public class ModuleRoutePair
{
	/// <summary>
	///       The route base path.
	///       </summary>
	public string RouteBasePath { get; private set; }

	/// <summary>
	///       The route default values.
	///       </summary>
	public IDictionary<string, string> RouteDefaults { get; private set; }

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Modules.ModuleRoutePair" /> class.
	///       </summary>
	/// <param name="routeBasePath">The route base path.</param>
	/// <param name="routeDefaults">The route defaults.</param>
	public ModuleRoutePair(string routeBasePath, IDictionary<string, string> routeDefaults)
	{
		RouteBasePath = routeBasePath;
		RouteDefaults = routeDefaults;
	}
}
