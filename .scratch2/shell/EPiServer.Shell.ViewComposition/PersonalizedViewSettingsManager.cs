using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Provides methods to work with a user's view settings.
///       </summary>
public class PersonalizedViewSettingsManager
{
	private readonly IViewManager _viewManager;

	/// <summary>
	///       Gets the repository.
	///       </summary>
	/// <value>The repository.</value>
	public IPersonalizedViewSettingsRepository Repository { get; }

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.PersonalizedViewSettingsManager" /> class.
	///       </summary>
	/// <param name="repository">The personalized view settings repository.</param>
	/// <param name="viewManager">The view manager.</param>
	public PersonalizedViewSettingsManager(IPersonalizedViewSettingsRepository repository, IViewManager viewManager)
	{
		Repository = repository;
		_viewManager = viewManager;
	}

	/// <summary>
	///       Either get's an already existing personalized component or personalizes the component hierarchy and adds it to the personalized view settings object.
	///       </summary>
	/// <param name="principal">The principal.</param>
	/// <param name="viewName">Name of the view.</param>
	/// <param name="containerId">The container's id.</param>
	/// <returns>
	///       A personalized <see cref="T:EPiServer.Shell.ViewComposition.IComponent" /> object.
	///       </returns>
	public IComponent GetOrCreateCustomizedComponent(IPrincipal principal, string viewName, Guid containerId)
	{
		PersonalizedViewSettings personalizedViewSettings;
		IContainer personalizationContainer;
		return GetOrCreatePersonalizedComponent(principal, viewName, containerId, out personalizedViewSettings, out personalizationContainer);
	}

	/// <summary>
	///       Either get's an already existing personalized component or creates a new one and adds it to the personalized view settings object.
	///       </summary>
	/// <param name="principal">The principal.</param>
	/// <param name="viewName">Name of the view.</param>
	/// <param name="componentId">The components's id.</param>
	/// <param name="personalizedViewSettings">A reference to the personalized view settings object that the component is placed in or null.</param>
	/// <param name="personalizationContainer">The personalization container is one exists or null.</param>
	/// <returns>
	///       A personalized <see cref="T:EPiServer.Shell.ViewComposition.IComponent" /> object.
	///       </returns>
	public IComponent GetOrCreatePersonalizedComponent(IPrincipal principal, string viewName, Guid componentId, out PersonalizedViewSettings personalizedViewSettings, out IContainer personalizationContainer)
	{
		ArgumentNullException.ThrowIfNull(principal, "principal");
		ArgumentNullException.ThrowIfNull(viewName, "viewName");
		personalizedViewSettings = GetUserSettings(viewName, principal, createIfItemDoesNotExist: true);
		IComponent component = personalizedViewSettings.FindComponentById(componentId, out personalizationContainer);
		if (component != null)
		{
			return component;
		}
		IComponent result = (_viewManager.CreateView(viewName, principal) ?? throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Could not find a view named \"{0}\"", viewName))).RootContainer.FindComponentById(componentId, ref personalizationContainer) ?? throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Could not find a component with id  \"{0}\" for view \"{1}\"", componentId, viewName));
		if (personalizationContainer == null)
		{
			throw new ViewCompositionException(string.Format(CultureInfo.InvariantCulture, "The component {0} of type {1} with id {2} can not be added since no parent container was found with personalization enabled.", component.DefinitionName, component.GetType().FullName, componentId));
		}
		personalizedViewSettings.CustomizedContainers.Add(personalizationContainer);
		return result;
	}

	/// <summary>
	///       Get the persolized settings for the user.
	///       </summary>
	/// <param name="viewName">The view to get the settings of.</param>
	/// <param name="principal">The user.</param>
	/// <param name="createIfItemDoesNotExist">if set to <c>true</c> a new item will be returned if there is no matching item in the backing repository.</param>
	/// <returns>
	///       The <see cref="T:EPiServer.Shell.ViewComposition.PersonalizedViewSettings" /> for the specified user and view.
	///       </returns>
	public PersonalizedViewSettings GetUserSettings(string viewName, IPrincipal principal, bool createIfItemDoesNotExist)
	{
		PersonalizedViewSettings personalizedViewSettings = Repository.Load(principal, viewName);
		if ((personalizedViewSettings == null) & createIfItemDoesNotExist)
		{
			personalizedViewSettings = new PersonalizedViewSettings
			{
				UserName = principal.Identity.Name,
				ViewName = viewName
			};
		}
		return personalizedViewSettings;
	}

	/// <summary>
	///       Deletes a component with a given id for a specific user and view.
	///       </summary>
	/// <param name="principal">The principal.</param>
	/// <param name="viewName">Name of the view.</param>
	/// <param name="id">The id of the component.</param>
	public void DeleteComponent(IPrincipal principal, string viewName, Guid id)
	{
		PersonalizedViewSettings userSettings = GetUserSettings(viewName, principal, createIfItemDoesNotExist: true);
		if (userSettings.FindComponentById(id, out var personalizationContainer) != null)
		{
			IContainer container = userSettings.CustomizedContainers.FirstOrDefault((IContainer c) => c.Id == id);
			if (container != null)
			{
				userSettings.CustomizedContainers.Remove(container);
			}
			else
			{
				personalizationContainer.RemoveComponentsRecursive(new List<IComponentMatcher>
				{
					new ComponentIdMatcher(id)
				}, notifyComponentOnRemoval: true);
			}
			Repository.Save(userSettings);
		}
	}
}
