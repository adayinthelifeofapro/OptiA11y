using System.Collections.Generic;
using EPiServer.ServiceLocation;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       The view configurations.
///       </summary>
[Options(ConfigurationSection = "CmsUI")]
public class ViewOptions
{
	/// <summary>
	///       The configurations for each view.
	///       </summary>
	public IList<ViewDetails> Views { get; } = new List<ViewDetails>();
}
