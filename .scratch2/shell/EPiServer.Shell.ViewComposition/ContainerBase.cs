using System;
using System.Collections.Generic;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Serves as a base for layout containers.
///       </summary>
public abstract class ContainerBase : ComponentBase, IContainer, IComponent, ICloneable
{
	private readonly List<IComponent> _components = new List<IComponent>();

	/// <summary>
	///       Gets or sets the components.
	///       </summary>
	/// <value>The components.</value>
	public IList<IComponent> Components => _components;

	/// <summary>
	///       Gets or sets the type of the container.
	///       </summary>
	/// <value>The type of the container.</value>
	public ContainerType ContainerType { get; set; }

	/// <summary>
	///       The plug-in area that is used to plug-in components automatically.
	///       </summary>
	public string PlugInArea { get; set; }

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.ContainerBase" /> class.
	///       </summary>
	protected ContainerBase(string widgetType)
		: base(widgetType)
	{
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.ContainerBase" /> class.
	///       </summary>
	/// <param name="widgetType">Type of the widget.</param>
	/// <param name="settings">The settings.</param>
	protected ContainerBase(string widgetType, ISettingsDictionary settings)
		: base(widgetType, settings)
	{
	}

	/// <summary>
	///       Adds a component to the collection.
	///       </summary>
	/// <param name="component">The component.</param>
	/// <returns>
	/// </returns>
	public IContainer Add(IComponent component)
	{
		_components.Add(component);
		return this;
	}

	/// <summary>
	///       Sort the components using the specified comparison delegate.
	///       </summary>
	/// <param name="comparison">The comparison delegate determining the new component order.</param>
	public void SortComponents(Comparison<IComponent> comparison)
	{
		_components.Sort(comparison);
	}

	/// <summary>
	///       Creates a new object that is a copy of the current instance.
	///       </summary>
	/// <returns>
	///       A new object that is a copy of this instance.
	///       </returns>
	public object Clone()
	{
		ContainerBase obj = (ContainerBase)MemberwiseClone();
		obj.Id = Guid.NewGuid();
		return obj;
	}
}
/// <summary>
///       Inherit ContainerBase to create a container that needs it's children to have a specific kind of settings added to them
///       </summary>
/// <typeparam name="TSettings">The type of the setting.</typeparam>
public abstract class ContainerBase<TSettings> : ContainerBase, IContainer<TSettings>, IContainer, IComponent, ICloneable where TSettings : ISettingsDictionary
{
	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.ContainerBase`1" /> class.
	///       </summary>
	/// <param name="widgetType">
	/// </param>
	protected ContainerBase(string widgetType)
		: base(widgetType)
	{
	}

	/// <summary>
	///       Adds the specified component.
	///       </summary>
	/// <param name="component">The component.</param>
	public new virtual IContainer Add(IComponent component)
	{
		throw new NotSupportedException("This container type needs a setting of type: " + typeof(TSettings).Name + " to be specified for all components added");
	}

	/// <summary>
	///       Adds the specified component.
	///       </summary>
	/// <param name="component">The component.</param>
	/// <param name="requiredSettings">The required settings.</param>
	/// <returns>Itself</returns>
	public virtual IContainer<TSettings> Add(IComponent component, TSettings requiredSettings)
	{
		foreach (KeyValuePair<string, object> item in requiredSettings)
		{
			component.Settings[item.Key] = item.Value;
		}
		base.Add(component);
		return this;
	}
}
