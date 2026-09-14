using System.Security.Principal;
using EPiServer.Framework;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Interface that is used to modify the plug-ins and settings for a view.
///       </summary>
public interface IViewTransformer : ISortable
{
	/// <summary>
	///       Transforms the view according to the rules for the transformer.
	///       </summary>
	/// <param name="view">The view.</param>
	/// <param name="principal">The principal.</param>
	void TransformView(ICompositeView view, IPrincipal principal);
}
