using System.Collections.Generic;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
/// </summary>
public interface IViewProvider
{
	/// <summary>
	///       Gets the views.
	///       </summary>
	/// <returns>A collection of views</returns>
	IEnumerable<ICompositeView> GetViews();
}
