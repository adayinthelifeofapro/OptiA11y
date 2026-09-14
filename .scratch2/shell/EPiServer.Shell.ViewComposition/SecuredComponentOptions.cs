using System.Collections.Generic;
using EPiServer.ServiceLocation;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       The secured components configuration.
///       </summary>
[Options(ConfigurationSection = "CmsUI")]
public class SecuredComponentOptions
{
	/// <summary>
	///       Components that require specific access rights.
	///       </summary>
	/// <remarks>This can also be used to disable components for all users.</remarks>
	public IList<SecuredComponentDetails> Components { get; } = new List<SecuredComponentDetails>();
}
