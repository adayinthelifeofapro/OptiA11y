using System.Collections.Generic;
using EPiServer.ServiceLocation;

namespace EPiServer.Shell.Navigation;

/// <summary>
///       A collection of modules. In addition to modules configured in the
///       collection modules will be auto-discovered from the resource root path.
///       </summary>
[Options(ConfigurationSection = "CmsUI")]
public class NavigationOptions
{
	/// <summary>
	///       Gets all the modules item for the current instance of module
	///       </summary>
	public IList<NavigationDetails> Items { get; } = new List<NavigationDetails>();
}
