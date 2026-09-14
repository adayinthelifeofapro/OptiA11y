using System.Collections.Generic;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Represents the details of the view inside episerver.shell.viewManager configuration.
///       </summary>
public class ViewDetails
{
	/// <summary>
	///       The name of the view details.
	///       </summary>
	public string Name { get; set; }

	/// <summary>
	///       The transformation settings.
	///       </summary>
	public IList<ViewTransformationDetails> TransformationSettings { get; } = new List<ViewTransformationDetails>();
}
