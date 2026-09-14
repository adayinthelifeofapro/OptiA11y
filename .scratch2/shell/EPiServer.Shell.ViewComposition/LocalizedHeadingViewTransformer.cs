using System.Security.Principal;
using EPiServer.Framework;
using EPiServer.Framework.Localization;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
/// </summary>
[ViewTransformer]
internal class LocalizedHeadingViewTransformer : IViewTransformer, ISortable
{
	private readonly LocalizationService _localizationService;

	/// <summary>
	///       Used to determine the execution order when there are several <see cref="T:EPiServer.Shell.ViewComposition.IViewTransformer" /> classes.
	///       </summary>
	public int SortOrder => 15000;

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.LocalizedHeadingViewTransformer" /> class.
	///       </summary>
	/// <param name="localizationService">The localization service.</param>
	public LocalizedHeadingViewTransformer(LocalizationService localizationService)
	{
		_localizationService = localizationService;
	}

	/// <summary>
	///       Transforms the view according to the rules for the transformer.
	///       </summary>
	/// <param name="view">The view.</param>
	/// <param name="principal">The principal.</param>
	public void TransformView(ICompositeView view, IPrincipal principal)
	{
		TransformViewRecursively(view.RootContainer);
	}

	private void TransformViewRecursively(IComponent component)
	{
		if (component is ILocalizedHeadingComponent localizedHeadingComponent && !string.IsNullOrEmpty(localizedHeadingComponent.HeadingLocalizationKey))
		{
			localizedHeadingComponent.Heading = _localizationService.GetString(localizedHeadingComponent.HeadingLocalizationKey);
		}
		if (!(component is IContainer { Components: not null } container))
		{
			return;
		}
		foreach (IComponent component2 in container.Components)
		{
			TransformViewRecursively(component2);
		}
	}
}
