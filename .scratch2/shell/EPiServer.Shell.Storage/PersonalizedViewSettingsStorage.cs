using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using EPiServer.Data.Dynamic;
using EPiServer.ServiceLocation;
using EPiServer.Shell.ViewComposition;
using Microsoft.Extensions.Logging;

namespace EPiServer.Shell.Storage;

/// <summary>
///       Class that is used to store personalized settings.
///       This class supports the EPiServer infrastructure and is not intended to be used directly from your code.
///       </summary>
/// <remarks>This class converts <see cref="T:EPiServer.Shell.ViewComposition.IContainer" /> and <see cref="T:EPiServer.Shell.ViewComposition.IComponent" /> to <see cref="T:EPiServer.Shell.Storage.ComponentData" /> to be able to store just the data needed.</remarks>
[EPiServerDataTable(TableName = "tblSystemBigTable")]
internal class PersonalizedViewSettingsStorage
{
	private readonly ILogger<PersonalizedViewSettingsRepository> _log;

	private readonly IComponentManager _componentManager;

	[CompilerGenerated]
	private IList<ComponentData> _003CCustomizedContainers_003Ek__BackingField;

	/// <summary>
	///       Id for the personalized settings.
	///       </summary>
	public Guid Id { get; set; }

	/// <summary>
	///       Gets or sets the name of the view.
	///       </summary>
	/// <value>The name of the view.</value>
	[EPiServerDataIndex]
	public string ViewName { get; set; }

	/// <summary>
	///       Gets or sets the username for these settings.
	///       </summary>
	/// <value>The username for these settings.</value>
	[EPiServerDataIndex]
	public string UserName { get; set; }

	/// <summary>
	///       Gets or sets the internal collection that is used for storage.
	///       </summary>
	/// <value>The internal collection that is used for storage.</value>
	public IList<ComponentData> CustomizedContainers
	{
		get
		{
			if (_003CCustomizedContainers_003Ek__BackingField == null)
			{
				_003CCustomizedContainers_003Ek__BackingField = new List<ComponentData>();
			}
			return _003CCustomizedContainers_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CCustomizedContainers_003Ek__BackingField = value;
		}
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.PersonalizedViewSettings" /> class.
	///       </summary>
	public PersonalizedViewSettingsStorage()
		: this(ServiceProviderExtensions.GetInstance<IComponentManager>(ServiceLocator.Current), ServiceProviderExtensions.GetInstance<ILogger<PersonalizedViewSettingsRepository>>(ServiceLocator.Current))
	{
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.PersonalizedViewSettings" /> class.
	///       </summary>
	/// <param name="componentManager">The component manager.</param>
	/// <param name="logger">The logger.</param>
	public PersonalizedViewSettingsStorage(IComponentManager componentManager, ILogger<PersonalizedViewSettingsRepository> logger)
	{
		_componentManager = componentManager;
		_log = logger;
	}

	/// <summary>
	///       Creates from new instance of <see cref="T:EPiServer.Shell.Storage.PersonalizedViewSettingsStorage" /> with the settings from the personalized view settings.
	///       </summary>
	/// <param name="settings">The settings.</param>
	/// <param name="manager">The <see cref="T:EPiServer.Shell.ViewComposition.IComponentManager" /> that is responsible for creating components.</param>
	/// <param name="logger">The logger.</param>
	/// <returns>A new instance of <see cref="T:EPiServer.Shell.Storage.PersonalizedViewSettingsStorage" />.</returns>
	public static PersonalizedViewSettingsStorage CreateFromPersonalizedViewSettings(PersonalizedViewSettings settings, IComponentManager manager, ILogger<PersonalizedViewSettingsRepository> logger)
	{
		PersonalizedViewSettingsStorage personalizedViewSettingsStorage = new PersonalizedViewSettingsStorage(manager, logger)
		{
			Id = settings.Id,
			UserName = settings.UserName,
			ViewName = settings.ViewName
		};
		ConvertIContainersToDataContainer(settings.CustomizedContainers, personalizedViewSettingsStorage.CustomizedContainers);
		return personalizedViewSettingsStorage;
	}

	/// <summary>
	///       Converts this instance to the public format (<see cref="T:EPiServer.Shell.ViewComposition.PersonalizedViewSettings" />).
	///       </summary>
	/// <returns>A <see cref="T:EPiServer.Shell.ViewComposition.PersonalizedViewSettings" /> instance with the settings from this instance.</returns>
	public PersonalizedViewSettings ConvertToPersonalizedViewSettings(IPrincipal principal)
	{
		PersonalizedViewSettings personalizedViewSettings = new PersonalizedViewSettings
		{
			Id = Id,
			UserName = UserName,
			ViewName = ViewName
		};
		ConvertDataContainersToIContainer(CustomizedContainers, personalizedViewSettings.CustomizedContainers, principal);
		return personalizedViewSettings;
	}

	private void ConvertDataContainersToIContainer(IEnumerable<ComponentData> source, IList<IContainer> destination, IPrincipal principal)
	{
		foreach (ComponentData item in source)
		{
			if (ConvertComponentDataToIComponent(item, principal) is IContainer container)
			{
				container.ContainerType = ContainerType.User;
				destination.Add(container);
			}
		}
	}

	private void ConvertDataContainersToIContainer(IEnumerable<ComponentData> source, IContainer destination, IPrincipal principal)
	{
		foreach (ComponentData item in source)
		{
			IComponent component = ConvertComponentDataToIComponent(item, principal);
			if (component != null)
			{
				destination.Components.Add(component);
			}
		}
	}

	private IComponent ConvertComponentDataToIComponent(ComponentData source, IPrincipal principal)
	{
		IComponent component;
		try
		{
			component = _componentManager.CreateComponent(source.DefinitionName, principal);
		}
		catch (UnauthorizedAccessException)
		{
			return null;
		}
		if (component == null)
		{
			if (_log.IsEnabled(LogLevel.Warning))
			{
				_log.CouldNotLoadPersonalizedComponent(source.DefinitionName, ViewName);
			}
			return null;
		}
		component.Id = source.Id;
		component.Settings.AddSettings(source.Settings, personalizable: true);
		if (!(component is IContainer container))
		{
			return component;
		}
		container.PlugInArea = source.PlugInArea;
		ConvertDataContainersToIContainer(source.Components, container, principal);
		return component;
	}

	private static void ConvertIComponentsToDataComponents(IEnumerable<IComponent> source, IList<ComponentData> destination)
	{
		foreach (IComponent item in source)
		{
			ConvertIComponentToDataComponent(destination, item);
		}
	}

	private static void ConvertIContainersToDataContainer(IEnumerable<IContainer> source, IList<ComponentData> destination)
	{
		foreach (IContainer item in source)
		{
			ConvertIComponentToDataComponent(destination, item);
		}
	}

	private static void ConvertIComponentToDataComponent(IList<ComponentData> destination, IComponent component)
	{
		ComponentData componentData = CreateStorageData(component);
		destination.Add(componentData);
		if (component is IContainer container)
		{
			ConvertIComponentsToDataComponents(container.Components, componentData.Components);
		}
	}

	private static ComponentData CreateStorageData(IComponent source)
	{
		return new ComponentData
		{
			Id = source.Id,
			DefinitionName = source.DefinitionName,
			Settings = source.Settings.GetPersonalizableSettings(),
			PlugInArea = ((!(source is IContainer container)) ? null : container.PlugInArea)
		};
	}
}
