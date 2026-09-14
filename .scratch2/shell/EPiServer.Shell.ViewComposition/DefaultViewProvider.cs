using System.Collections.Generic;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Returns all <see cref="T:EPiServer.Shell.ViewComposition.ICompositeView" /> found.
///       </summary>
[ViewProvider]
public class DefaultViewProvider : IViewProvider
{
	private readonly IEnumerable<ICompositeView> _views;

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.DefaultViewProvider" /> class.
	///       </summary>
	/// <param name="views">The views for the provider to return.</param>
	public DefaultViewProvider(IEnumerable<ICompositeView> views)
	{
		_views = views;
	}

	/// <summary>
	///       Gets the views.
	///       </summary>
	/// <returns>
	/// </returns>
	public IEnumerable<ICompositeView> GetViews()
	{
		return _views;
	}
}
