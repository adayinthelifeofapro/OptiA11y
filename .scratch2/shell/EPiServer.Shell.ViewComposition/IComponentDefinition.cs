using System;
using System.Collections.Generic;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Defines information about a component. This can be used when creating lists of selectable components for a user.
///       </summary>
public interface IComponentDefinition : IPluggableComponentDefinition, IContainerMatcher, IEquatable<IComponentDefinition>
{
	/// <summary>
	///       Unique name of the component definition.
	///       </summary>
	/// <value>The unique name of the component definition.</value>
	/// <remarks>This is used when creating component from a <see cref="T:EPiServer.Shell.ViewComposition.IComponentProvider" />. For instance, it can use the <see cref="P:System.Type.FullName" /> of the type of the <see cref="T:EPiServer.Shell.ViewComposition.IComponentDefinition" />.</remarks>
	string DefinitionName { get; }

	/// <summary>
	///       Name of the widget type to use for displaying the component.
	///       </summary>
	/// <value>The name of the widget type to use for displaying the component.</value>
	string WidgetType { get; }

	/// <summary>
	///       Title used in the UI both for listings and as default for component instances. Should be localized.
	///       </summary>
	/// <value>The title.</value>
	string Title { get; }

	/// <summary>
	///       Description of the component. Should be localized.
	///       </summary>
	/// <value>The description.</value>
	string Description { get; }

	/// <summary>
	///       Gets or sets the category for this component. For example "cms".
	///       </summary>
	/// <value>The category.</value>
	IEnumerable<string> Categories { get; }
}
