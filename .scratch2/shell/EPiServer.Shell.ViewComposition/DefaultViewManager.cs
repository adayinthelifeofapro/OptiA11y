using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using EPiServer.Framework;
using EPiServer.Shell.Modules;
using EPiServer.Web.Routing;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Handles the finding and composing of <see cref="T:EPiServer.Shell.ViewComposition.ICompositeView" /> with <see cref="T:EPiServer.Shell.ViewComposition.IComponent" />.
///       </summary>
public class DefaultViewManager : IViewManager
{
	private readonly IEnumerable<IViewProvider> _viewProviders;

	private readonly List<IViewTransformer> _viewTransformers;

	private readonly IComponentManager _componentManager;

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.DefaultViewManager" /> class.
	///       </summary>
	/// <param name="viewProviders">The view providers.</param>
	/// <param name="viewTransformers">The view transformers used when transforming the views returned.</param>
	/// <param name="componentManager">The component manager providing components for populating the views.</param>
	public DefaultViewManager(IEnumerable<IViewProvider> viewProviders, IEnumerable<IViewTransformer> viewTransformers, IComponentManager componentManager)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Expected O, but got Unknown
		base._002Ector();
		_viewProviders = viewProviders;
		_componentManager = componentManager;
		if (viewTransformers == null)
		{
			_viewTransformers = new List<IViewTransformer>();
			return;
		}
		_viewTransformers = viewTransformers.ToList();
		_viewTransformers.Sort((IComparer<IViewTransformer>?)new SortableComparer());
	}

	/// <summary>
	///       Gets the view name.
	///       </summary>
	/// <param name="shellModule">the module</param>
	/// <param name="urlSegment">the segment to match</param>
	/// <returns>The view name if exist, otherwise null.</returns>
	public ICompositeView GetView(ShellModule shellModule, string urlSegment)
	{
		return _viewProviders.Select((IViewProvider provider) => provider.GetViews().OfType<IRoutable>().FirstOrDefault((IRoutable x) => x.RouteSegment.Equals(urlSegment, StringComparison.OrdinalIgnoreCase) && shellModule.Assemblies.Contains(((object)x).GetType().Assembly))).OfType<ICompositeView>().FirstOrDefault();
	}

	/// <summary>
	///       Gets the view.
	///       </summary>
	/// <param name="viewName">The name of the view.</param>
	/// <param name="principal">The principal.</param>
	/// <returns>An <see cref="T:EPiServer.Shell.ViewComposition.ICompositeView" /> populated with <see cref="T:EPiServer.Shell.ViewComposition.IComponent" />s.</returns>
	public ICompositeView CreateView(string viewName, IPrincipal principal)
	{
		ICompositeView compositeView = null;
		foreach (IViewProvider viewProvider in _viewProviders)
		{
			compositeView = viewProvider.GetViews().FirstOrDefault((ICompositeView v) => v.Name.Equals(viewName, StringComparison.OrdinalIgnoreCase));
			if (compositeView != null)
			{
				compositeView = compositeView.CreateView();
				break;
			}
		}
		if (compositeView != null && compositeView.RootContainer != null)
		{
			compositeView.RootContainer.AddComponentsRecursive(viewName, (from d in _componentManager.ListAll()
				where d.SupportsAutomaticRegistration
				select d).ToArray(), principal);
			foreach (IViewTransformer viewTransformer in _viewTransformers)
			{
				viewTransformer.TransformView(compositeView, principal);
			}
		}
		return compositeView;
	}
}
