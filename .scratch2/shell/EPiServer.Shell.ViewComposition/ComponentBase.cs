using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Modules;
using Microsoft.Extensions.Logging;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Serves as a base for layout containers
///       </summary>
public abstract class ComponentBase : IComponent
{
	private string _moduleName;

	[CompilerGenerated]
	private ISettingsDictionary _003CSettings_003Ek__BackingField;

	/// <summary>
	///       The <see cref="T:System.Guid" /> of the item
	///       </summary>
	public Guid Id { get; set; }

	/// <summary>
	///       Gets or sets a heading used client side for the component.
	///       </summary>
	public string Heading
	{
		get
		{
			return Settings["heading"] as string;
		}
		set
		{
			Settings["heading"] = value;
		}
	}

	/// <summary>
	///       Unique name of the component type.
	///       </summary>
	/// <value>The unique name that is used to create new <see cref="T:EPiServer.Shell.ViewComposition.IComponent" />s from the <see cref="T:EPiServer.Shell.ViewComposition.IComponentProvider" />s.</value>
	public virtual string DefinitionName => GetType().FullName;

	/// <summary>
	///       Name of the widget type to use for displaying the component.
	///       </summary>
	/// <value>The name of the widget type to use for displaying the component.</value>
	public string WidgetType { get; private set; }

	/// <inheritdoc />
	public virtual string ModuleName => _moduleName ?? (_moduleName = GetModuleName());

	/// <summary>
	///       Gets or sets the sort order for this component.
	///       </summary>
	public int SortOrder { get; set; } = 100;

	/// <summary>
	///       Gets the settings collection for this component. Used for client activation and persisted on the server.
	///       </summary>
	public ISettingsDictionary Settings => _003CSettings_003Ek__BackingField ?? (_003CSettings_003Ek__BackingField = new SettingsDictionary());

	/// <summary>
	///       Initializes and sets as active new instance of the <see cref="T:EPiServer.Shell.ViewComposition.ComponentBase" /> class.
	///       </summary>
	protected ComponentBase(string widgetType)
		: this(widgetType, Guid.NewGuid(), null)
	{
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.ComponentBase" /> class.
	///       </summary>
	/// <param name="widgetType">Type of the widget.</param>
	/// <param name="settings">The initial settings.</param>
	protected ComponentBase(string widgetType, ISettingsDictionary settings)
		: this(widgetType, Guid.NewGuid(), settings)
	{
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.ComponentBase" /> class.
	///       </summary>
	/// <param name="widgetType">Type of the widget.</param>
	/// <param name="id">The id.</param>
	/// <param name="settings">The initial settings.</param>
	protected ComponentBase(string widgetType, Guid id, ISettingsDictionary settings)
	{
		Id = id;
		WidgetType = widgetType;
		Settings.Add(new PersonalizableSetting("column", 0));
		Settings.Add(new Setting("lastOpenHeight", personalizable: true));
		Settings.Add(new Setting("open", true, personalizable: true));
		if (settings == null)
		{
			return;
		}
		IDictionary<string, object> personlizable = settings.GetPersonalizableSettings();
		personlizable?.ToList().ForEach(delegate(KeyValuePair<string, object> s)
		{
			Settings.Add(new Setting(s.Key, s.Value, personalizable: true));
		});
		settings.ToList().ForEach(delegate(KeyValuePair<string, object> s)
		{
			if (!personlizable.Any((KeyValuePair<string, object> p) => p.Key == s.Key))
			{
				Settings.Add(new Setting(s.Key, s.Value));
			}
		});
	}

	/// <summary>
	///       Gets the name of the module.
	///       </summary>
	/// <returns>
	/// </returns>
	protected virtual string GetModuleName()
	{
		return GetModuleName(GetType());
	}

	/// <summary>
	///       Returns a ShellModule to which a specified Type belongs.
	///       </summary>
	/// <param name="type">The type.</param>
	/// <returns>
	/// </returns>
	protected string GetModuleName(Type type)
	{
		ILogger<ModuleTable> instance = ServiceProviderExtensions.GetInstance<ILogger<ModuleTable>>(ServiceLocator.Current);
		ModuleTable moduleTable = default(ModuleTable);
		(ServiceProviderExtensions.TryGetExistingInstance<ModuleTable>(ServiceLocator.Current, ref moduleTable) ? moduleTable : new ModuleTable(instance)).TryGetModule(type.Assembly, out var shellModule);
		if (shellModule != null)
		{
			return shellModule.Name;
		}
		return string.Empty;
	}
}
