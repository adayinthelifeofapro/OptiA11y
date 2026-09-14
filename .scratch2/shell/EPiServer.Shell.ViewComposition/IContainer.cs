using System;
using System.Collections.Generic;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Defines that a component might have child components.
///       </summary>
public interface IContainer : IComponent, ICloneable
{
	/// <summary>
	///       Gets the child components.
	///       </summary>
	/// <value>The child components.</value>
	IList<IComponent> Components { get; }

	/// <summary>
	///       Gets or sets the <see cref="P:EPiServer.Shell.ViewComposition.IContainer.ContainerType" /> indicating if it should be handled as  
	///       shared system instances or saved per user.
	///       </summary>
	/// <value>The type of the container.</value>
	ContainerType ContainerType { get; set; }

	/// <summary>
	///       The plug-in path that is used to plug-in components automatically.
	///       </summary>
	string PlugInArea { get; set; }

	/// <summary>
	///       Adds a component to the collection and returns the container to allow chaining.
	///       </summary>
	/// <param name="component">The component.</param>
	IContainer Add(IComponent component);

	/// <summary>
	///       Sort the components using the specified comparison delegate.
	///       </summary>
	/// <param name="comparison">The comparison delegate determining the new component order.</param>
	void SortComponents(Comparison<IComponent> comparison);
}
/// <summary>
///       Defines that a component might have child components.
///       </summary>
public interface IContainer<in TSettings> : IContainer, IComponent, ICloneable where TSettings : ISettingsDictionary
{
	/// <summary>
	///       Adds a component to the collection.
	///       </summary>
	/// <param name="component">The component.</param>
	/// <param name="requiredSettings">The required setting.</param>
	/// <returns>Itself</returns>
	IContainer<TSettings> Add(IComponent component, TSettings requiredSettings);
}
