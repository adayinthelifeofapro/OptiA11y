using System;

namespace EPiServer.Shell.Dashboard.Internal;

public interface IDashboardGadgetBase
{
	Guid Guid { get; set; }

	/// <summary>
	///       Gadget's unique name
	///       </summary>
	string DisplayName { get; set; }

	/// <summary>
	///       Gadget's localization path
	///       </summary>
	string LanguagePath { get; set; }

	/// <summary>
	///       Gets or sets authorization policy name. If set, only users with proper permissions can see this gadget.
	///       </summary>
	string AuthorizationPolicy { get; set; }
}
