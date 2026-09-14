using System.Security.Principal;
using EPiServer.Framework;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///           Sorts all components according to their sort order.
///       </summary>
[ViewTransformer]
public class SortComponentTransformer : IViewTransformer, ISortable
{
	/// <summary>
	///       Used to select the order of execution when there are several <see cref="T:EPiServer.Shell.ViewComposition.IViewTransformer" />s.
	///       </summary>
	public int SortOrder => 8000;

	/// <summary>
	///       Transforms the view according to the rules for the transformer.
	///       </summary>
	/// <param name="view">The view.</param>
	/// <param name="principal">The principal.</param>
	public void TransformView(ICompositeView view, IPrincipal principal)
	{
		SortContainerRecursively(view.RootContainer);
	}

	private void SortContainerRecursively(IContainer parentContainer)
	{
		foreach (IComponent component in parentContainer.Components)
		{
			if (component is IContainer parentContainer2)
			{
				SortContainerRecursively(parentContainer2);
			}
		}
		parentContainer.SortComponents((IComponent c1, IComponent c2) => c1.SortOrder.CompareTo(c2.SortOrder));
	}
}
