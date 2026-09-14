using System;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Defines the settings for a component.
///       </summary>
public interface IComponent
{
	/// <summary>
	///       Gets or sets the id.
	///       </summary>
	Guid Id { get; set; }

	/// <summary>
	///       Gets the settings collection for this component. Used for client activation and persisted on the server.
	///       </summary>
	ISettingsDictionary Settings { get; }

	/// <summary>
	///       Unique name of the component type.
	///       </summary>
	/// <value>The unique name that is used to create new <see cref="T:EPiServer.Shell.ViewComposition.IComponent" />s from the <see cref="T:EPiServer.Shell.ViewComposition.IComponentProvider" />s.</value>
	string DefinitionName { get; }

	/// <summary>
	///       Gets the widget type used when displaying the component.
	///       </summary>
	string WidgetType { get; }

	/// <summary>
	///       Gets the name of the Shell module to which the component belongs to.
	///       </summary>
	/// <value>
	///       The name of the Shell module.
	///       </value>
	string ModuleName { get; }

	/// <summary>
	///       Gets or sets the sort order for this component.
	///       </summary>
	int SortOrder { get; set; }
}
