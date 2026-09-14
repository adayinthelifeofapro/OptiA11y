using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using EPiServer.ServiceLocation;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Contains extension methods for <see cref="T:EPiServer.Shell.ViewComposition.IContainer" />.
///       </summary>
public static class IContainerExtensions
{
	/// <summary>
	///       Gets a list with all widget types currently in the containers component structure.
	///       </summary>
	/// <param name="container">The container.</param>
	/// <returns>
	///       A list with all widget types currently in the containers component structure.
	///       </returns>
	public static IEnumerable<string> GetComponentList(this IContainer container)
	{
		HashSet<string> hashSet = new HashSet<string>();
		GetComponentListRecursive(container, hashSet);
		return hashSet;
	}

	private static void GetComponentListRecursive(IContainer container, HashSet<string> typeList)
	{
		foreach (IComponent component in container.Components)
		{
			if (component is IContainer container2)
			{
				GetComponentListRecursive(container2, typeList);
			}
			if (!string.IsNullOrEmpty(component.WidgetType))
			{
				typeList.Add(component.WidgetType);
			}
		}
	}

	/// <summary>
	///       Tries to add the components to one of the containers in the given root container.
	///       </summary>
	/// <param name="container">The container.</param>
	/// <param name="viewName">The name of the view.</param>
	/// <param name="pluggableComponents">The components.</param>
	/// <param name="principal">The principal.</param>
	public static void AddComponentsRecursive(this IContainer container, string viewName, IEnumerable<IPluggableComponentDefinition> pluggableComponents, IPrincipal principal)
	{
		if (!string.IsNullOrEmpty(container.PlugInArea))
		{
			foreach (IPluggableComponentDefinition item in pluggableComponents.Where((IPluggableComponentDefinition plug) => plug.HasAccess(principal) && plug.MatchesContainer(container)))
			{
				container.Add(item.CreateComponent());
			}
		}
		foreach (IContainer item2 in container.Components.Where((IComponent c) => c is IContainer))
		{
			item2.AddComponentsRecursive(viewName, pluggableComponents, principal);
		}
	}

	/// <summary>
	///       Replaces all containers in the container tree with any container in the replacementContainer list that matches the plug-in path.
	///       </summary>
	/// <param name="container">The container to alter.</param>
	/// <param name="replacementContainers">The replacement containers.</param>
	/// <param name="componentManager">The component manager suppling components from component definitions.</param>
	/// <remarks>This method will never replace the root container, only children in the container tree.</remarks>
	public static void ReplaceContainersRecursive(this IContainer container, IEnumerable<IContainer> replacementContainers, IComponentManager componentManager)
	{
		if (replacementContainers != null)
		{
			ReplaceContainersRecursiveInternal(container, replacementContainers, componentManager);
		}
	}

	/// <summary>
	///       Replaces all containers in the container tree with any container in the replacementContainer list that matches the plug in path.
	///       </summary>
	/// <param name="container">The container to alter.</param>
	/// <param name="replacementContainers">The replacement containers.</param>
	/// <param name="componentManager">The component manager.</param>
	/// <returns>
	///       A replacement container that matches the plug in path of the given container.
	///       </returns>
	private static IContainer ReplaceContainersRecursiveInternal(IContainer container, IEnumerable<IContainer> replacementContainers, IComponentManager componentManager)
	{
		IContainer container2 = replacementContainers.FirstOrDefault((IContainer c) => c.PlugInArea == container.PlugInArea);
		if (container2 != null)
		{
			foreach (KeyValuePair<string, object> setting in container.Settings)
			{
				if (!container2.Settings.ContainsKey(setting.Key))
				{
					container2.Settings.Add(setting);
				}
			}
			return container2;
		}
		for (int num = container.Components.Count - 1; num >= 0; num--)
		{
			if (container.Components[num] is IContainer container3)
			{
				IContainer container4 = ReplaceContainersRecursiveInternal(container3, replacementContainers, componentManager);
				if (container4 != null)
				{
					container.Components[num] = container4;
				}
			}
		}
		return null;
	}

	/// <summary>
	///       Tries to remove any components in any of the containers in the given root container structure that matches the components plug-in path and type.
	///       </summary>
	/// <param name="container">The container.</param>
	/// <param name="componentMatchers">The component matchers.</param>
	/// <param name="notifyComponentOnRemoval">if set to <c>true</c> any component that implement <see cref="T:EPiServer.Shell.ViewComposition.IComponentNotify" /> are notified when removed.</param>
	public static void RemoveComponentsRecursive(this IContainer container, IEnumerable<IComponentMatcher> componentMatchers, bool notifyComponentOnRemoval)
	{
		foreach (IComponentMatcher item in componentMatchers.Where((IComponentMatcher matcher) => matcher.MatchesContainer(container)))
		{
			for (int num = container.Components.Count - 1; num >= 0; num--)
			{
				IComponent component = container.Components[num];
				if (item.MatchesComponent(component))
				{
					if (notifyComponentOnRemoval)
					{
						NotifyComponentsRecursive(component);
					}
					container.Components.Remove(component);
				}
			}
		}
		foreach (IContainer item2 in container.Components.Where((IComponent c) => c is IContainer))
		{
			item2.RemoveComponentsRecursive(componentMatchers, notifyComponentOnRemoval);
		}
	}

	/// <summary>
	///       Notifies the component and any child components that implement recursive.
	///       </summary>
	/// <param name="componentToRemove">The component to remove.</param>
	private static void NotifyComponentsRecursive(IComponent componentToRemove)
	{
		if (componentToRemove is IComponentNotify componentNotify)
		{
			componentNotify.OnDeleted(componentToRemove.Id);
		}
		if (!(componentToRemove is IContainer container))
		{
			return;
		}
		foreach (IComponent component in container.Components)
		{
			NotifyComponentsRecursive(component);
		}
	}

	/// <summary>
	///       Gets the first container in the container hierarchy that matches plug in path.
	///       </summary>
	/// <param name="container">The container.</param>
	/// <param name="plugInArea">The plug in area.</param>
	/// <returns>The first container in the container hierarchy that matches plug in path.</returns>
	public static IContainer FindContainerByPlugInArea(this IContainer container, string plugInArea)
	{
		if (string.Equals(container.PlugInArea, plugInArea, StringComparison.OrdinalIgnoreCase))
		{
			return container;
		}
		foreach (IContainer item in container.Components.Where((IComponent c) => c is IContainer))
		{
			IContainer container2 = item.FindContainerByPlugInArea(plugInArea);
			if (container2 != null)
			{
				return container2;
			}
		}
		return null;
	}

	/// <summary>
	///       Finds the component that matches the given id or null if no matching component exists.
	///       </summary>
	/// <param name="container">The container to search in.</param>
	/// <param name="id">The id to match.</param>
	/// <returns>
	///       The component that matches the given id or null if no matching container exists.
	///       </returns>
	public static IComponent FindComponentById(this IContainer container, Guid id)
	{
		IContainer personalizationContainer = null;
		return container.FindComponentById(id, ref personalizationContainer);
	}

	/// <summary>
	///       Finds the component that matches the given id or null if no matching component exists.
	///       </summary>
	/// <param name="container">The container to search in.</param>
	/// <param name="id">The id to match.</param>
	/// <param name="personalizationContainer">The personalization container.</param>
	/// <returns>
	///       The component that matches the given id or null if no matching container exists.
	///       </returns>
	public static IComponent FindComponentById(this IContainer container, Guid id, ref IContainer personalizationContainer)
	{
		if (container.Id == id)
		{
			if (personalizationContainer == null && container.ContainerType == ContainerType.User)
			{
				personalizationContainer = container;
			}
			return container;
		}
		foreach (IComponent component2 in container.Components)
		{
			if (!(component2 is IContainer container2))
			{
				if (component2.Id == id)
				{
					return component2;
				}
				continue;
			}
			IComponent component = container2.FindComponentById(id, ref personalizationContainer);
			if (component != null)
			{
				if (container2.ContainerType == ContainerType.User)
				{
					personalizationContainer = container2;
				}
				return component;
			}
		}
		return null;
	}

	internal static void AddAndSave(this IContainer container, IPersonalizedViewSettingsRepository settingsRepository, IComponentManager componentManager, ICompositeView view, IPrincipal principal, IComponent component)
	{
		PersonalizedViewSettings personalizedViewSettings = settingsRepository.Load(principal, view.Name);
		if (personalizedViewSettings != null)
		{
			view.RootContainer.ReplaceContainersRecursive(personalizedViewSettings.CustomizedContainers, componentManager);
			IContainer container2 = view.RootContainer.FindContainerByPlugInArea(container.PlugInArea);
			if (container2 != null)
			{
				container2.Add(component);
				settingsRepository.Save(personalizedViewSettings);
				return;
			}
		}
		container.Add(component);
	}

	/// <summary>
	///       Add component to view and save to the repository
	///       </summary>
	/// <param name="container">
	/// </param>
	/// <param name="view">
	/// </param>
	/// <param name="principal">
	/// </param>
	/// <param name="component">
	/// </param>
	public static void AddAndSave(this IContainer container, ICompositeView view, IPrincipal principal, IComponent component)
	{
		IPersonalizedViewSettingsRepository instance = ServiceProviderExtensions.GetInstance<IPersonalizedViewSettingsRepository>(ServiceLocator.Current);
		IComponentManager instance2 = ServiceProviderExtensions.GetInstance<IComponentManager>(ServiceLocator.Current);
		container.AddAndSave(instance, instance2, view, principal, component);
	}
}
