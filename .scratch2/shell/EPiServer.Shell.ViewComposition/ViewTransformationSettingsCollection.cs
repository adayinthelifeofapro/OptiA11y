using System.Collections.Generic;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Defines settings that can be used to modify the default behaviour and plug-ins for a view.
///       </summary>
public class ViewTransformationSettingsCollection : List<ViewTransformationSetting>
{
	/// <summary>
	///       Gets or sets the name of the view.
	///       </summary>
	/// <value>The name of the view.</value>
	public string ViewName { get; set; }
}
