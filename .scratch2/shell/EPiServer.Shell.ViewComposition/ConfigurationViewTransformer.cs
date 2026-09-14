using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Principal;
using EPiServer.Framework;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       ConfigurationViewTransformer is used to alter the default behaviour and components for a given view.
///       </summary>
public class ConfigurationViewTransformer : IViewTransformer, ISortable
{
	private class ConfigurationSettingsPlugIn : IPluggableComponentDefinition, IContainerMatcher
	{
		private readonly IComponentDefinition _definition;

		private readonly IComponentManager _componentManager;

		private readonly IPrincipal _principal;

		private readonly string _plugInArea;

		public bool SupportsAutomaticRegistration => true;

		public bool IsAvailableForUserSelection
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public ICollection<string> AllowedRoles => null;

		public ConfigurationSettingsPlugIn(IComponentDefinition definition, IComponentManager componentManager, IPrincipal principal, string plugInArea)
		{
			_definition = definition;
			_componentManager = componentManager;
			_principal = principal;
			_plugInArea = plugInArea;
		}

		public IComponent CreateComponent()
		{
			return _componentManager.CreateComponent(_definition, _principal);
		}

		public bool MatchesContainer(IContainer container)
		{
			return string.Equals(_plugInArea, container.PlugInArea, StringComparison.OrdinalIgnoreCase);
		}

		public bool HasAccess(IPrincipal principal)
		{
			return true;
		}
	}

	private class ConfigurationComponentMatcher : IComponentMatcher, IContainerMatcher
	{
		private readonly string _plugInArea;

		private readonly string _definitionName;

		public ConfigurationComponentMatcher(string plugInArea, string definitionName)
		{
			_plugInArea = plugInArea;
			_definitionName = definitionName;
		}

		public bool MatchesContainer(IContainer container)
		{
			return string.Equals(_plugInArea, container.PlugInArea, StringComparison.OrdinalIgnoreCase);
		}

		public bool MatchesComponent(IComponent component)
		{
			return string.Equals(_definitionName, component.DefinitionName, StringComparison.OrdinalIgnoreCase);
		}
	}

	private readonly IComponentManager _componentManager;

	/// <summary>
	///       Gets a collection of <see cref="T:EPiServer.Shell.ViewComposition.ViewTransformationSettingsCollection" /> for each view that has settings.
	///       </summary>
	/// <value>The settings.</value>
	public Collection<ViewTransformationSettingsCollection> Settings { get; private set; } = new Collection<ViewTransformationSettingsCollection>();

	/// <summary>
	///       Used to select the order of execution when there are several <see cref="T:EPiServer.Shell.ViewComposition.IViewTransformer" />s.
	///       </summary>
	/// <value>ConfigurationViewTransformer has 100.</value>
	public int SortOrder => 100;

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.ConfigurationViewTransformer" /> class.
	///       </summary>
	/// <param name="viewOptions">The <see cref="T:EPiServer.Shell.ViewComposition.ViewOptions" /> to read the configuration.</param>
	/// <param name="componentManager">The component manager.</param>
	public ConfigurationViewTransformer(ViewOptions viewOptions, IComponentManager componentManager)
	{
		_componentManager = componentManager;
		LoadSettingsFromConfiguration(viewOptions);
	}

	private void LoadSettingsFromConfiguration(ViewOptions viewOptions)
	{
		if (viewOptions == null)
		{
			return;
		}
		foreach (ViewDetails view in viewOptions.Views)
		{
			LoadSettingsFromConfiguration(view);
		}
	}

	/// <summary>
	///       Transforms the view according to the given settings.
	///       </summary>
	/// <param name="view">The view.</param>
	/// <param name="principal">The principal.</param>
	public void TransformView(ICompositeView view, IPrincipal principal)
	{
		ViewTransformationSettingsCollection viewTransformationSettingsCollection = Settings.FirstOrDefault((ViewTransformationSettingsCollection v) => v.ViewName.Equals(view.Name, StringComparison.OrdinalIgnoreCase));
		if (viewTransformationSettingsCollection == null)
		{
			return;
		}
		List<IPluggableComponentDefinition> list = new List<IPluggableComponentDefinition>();
		List<IComponentMatcher> list2 = new List<IComponentMatcher>();
		foreach (ViewTransformationSetting item in viewTransformationSettingsCollection)
		{
			switch (item.TransformationType)
			{
			case TransformationType.Add:
				AddSetting(list, item, _componentManager, principal);
				break;
			case TransformationType.Remove:
				AddSetting(list2, item);
				break;
			}
		}
		view.RootContainer.AddComponentsRecursive(view.Name, list, principal);
		view.RootContainer.RemoveComponentsRecursive(list2, notifyComponentOnRemoval: false);
	}

	private static void AddSetting(List<IPluggableComponentDefinition> components, ViewTransformationSetting setting, IComponentManager componentManager, IPrincipal principal)
	{
		ConfigurationSettingsPlugIn item = new ConfigurationSettingsPlugIn(componentManager.GetComponentDefinition(setting.DefinitionName), componentManager, principal, setting.PlugInArea);
		components.Add(item);
	}

	private static void AddSetting(List<IComponentMatcher> components, ViewTransformationSetting setting)
	{
		ConfigurationComponentMatcher item = new ConfigurationComponentMatcher(setting.PlugInArea, setting.DefinitionName);
		components.Add(item);
	}

	/// <summary>
	///       Loads the settings from configuration.
	///       </summary>
	/// <param name="viewDetails">The configuration object for one view.</param>
	public void LoadSettingsFromConfiguration(ViewDetails viewDetails)
	{
		if (viewDetails != null)
		{
			AddSettingFromConfiguration(viewDetails);
		}
	}

	private void AddSettingFromConfiguration(ViewDetails settingsElement)
	{
		ViewTransformationSettingsCollection viewTransformationSettingsCollection = Settings.FirstOrDefault((ViewTransformationSettingsCollection v) => v.ViewName.Equals(settingsElement.Name, StringComparison.OrdinalIgnoreCase));
		if (viewTransformationSettingsCollection == null)
		{
			viewTransformationSettingsCollection = new ViewTransformationSettingsCollection
			{
				ViewName = settingsElement.Name
			};
			Settings.Add(viewTransformationSettingsCollection);
		}
		foreach (ViewTransformationDetails transformationSetting in settingsElement.TransformationSettings)
		{
			ViewTransformationSetting item = new ViewTransformationSetting
			{
				TransformationType = transformationSetting.TransformationType,
				PlugInArea = transformationSetting.PlugInArea,
				DefinitionName = transformationSetting.DefinitionName
			};
			viewTransformationSettingsCollection.Add(item);
		}
	}
}
