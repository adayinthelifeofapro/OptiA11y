using System.Security.Principal;
using EPiServer.Framework;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Transforms a view according to a users settings.
///       </summary>
[ViewTransformer]
public class PersonalizationViewTransformer : IViewTransformer, ISortable
{
	private readonly IPersonalizedViewSettingsRepository _settingsRepository;

	private readonly IComponentManager _componentManager;

	/// <summary>
	///       Used to select the order of execution when there are several <see cref="T:EPiServer.Shell.ViewComposition.IViewTransformer" />s.
	///       </summary>
	/// <value>
	///   <see cref="T:EPiServer.Shell.ViewComposition.PersonalizationViewTransformer" /> has a sort order of 10000.</value>
	public int SortOrder => 10000;

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.PersonalizationViewTransformer" /> class.
	///       </summary>
	/// <param name="settingsRepository">The settings repository.</param>
	/// <param name="componentManager">The component manager.</param>
	public PersonalizationViewTransformer(IPersonalizedViewSettingsRepository settingsRepository, IComponentManager componentManager)
	{
		_settingsRepository = settingsRepository;
		_componentManager = componentManager;
	}

	/// <summary>
	///       Transforms the view according to the rules for the transformer.
	///       </summary>
	/// <param name="view">The view.</param>
	/// <param name="principal">The principal.</param>
	public void TransformView(ICompositeView view, IPrincipal principal)
	{
		PersonalizedViewSettings personalizedViewSettings = _settingsRepository.Load(principal, view.Name);
		if (personalizedViewSettings != null)
		{
			view.RootContainer.ReplaceContainersRecursive(personalizedViewSettings.CustomizedContainers, _componentManager);
		}
	}
}
