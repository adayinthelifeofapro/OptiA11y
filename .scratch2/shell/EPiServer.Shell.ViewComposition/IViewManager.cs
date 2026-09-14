using System.Security.Principal;
using EPiServer.Shell.Modules;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
/// </summary>
public interface IViewManager
{
	/// <summary>
	///       Creates the view.
	///       </summary>
	/// <param name="viewName">Name of the view.</param>
	/// <param name="principal">The user principal.</param>
	/// <returns>a view if found and allowed, otherwise null</returns>
	ICompositeView CreateView(string viewName, IPrincipal principal);

	/// <summary>
	///       Gets the view name.
	///       </summary>
	/// <param name="shellModule">the module</param>
	/// <param name="urlSegment">the segment to match</param>
	/// <returns>The view name if exist, otherwise null.</returns>
	ICompositeView GetView(ShellModule shellModule, string urlSegment);
}
