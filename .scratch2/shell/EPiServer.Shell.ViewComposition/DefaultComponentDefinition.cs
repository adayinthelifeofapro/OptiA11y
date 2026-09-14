using System;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       A component with settings that is defined elsewhere, for instance in a <see cref="T:EPiServer.Shell.ViewComposition.ComponentAttribute" />.
///       </summary>
internal class DefaultComponentDefinition : ComponentDefinitionBase
{
	private readonly Type _attributedComponentType;

	/// <inheritdoc />
	public override string DefinitionName => _attributedComponentType.FullName;

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.DefaultComponentDefinition" /> class.
	///       </summary>
	/// <param name="attributedComponentType">Type of the attributed component.</param>
	/// <param name="title">The title.</param>
	/// <param name="description">The description.</param>
	/// <param name="categories">The categories.</param>
	/// <param name="widgetType">Type of the widget.</param>
	/// <param name="languagePath">The path to the node in the localization resources.</param>
	/// <param name="plugInAreas">The plug in areas that the components should auto plug into.</param>
	/// <param name="sortOrder">The sort order.</param>
	public DefaultComponentDefinition(Type attributedComponentType, string title, string description, string[] categories, string widgetType, string languagePath, string[] plugInAreas, int sortOrder)
		: this(attributedComponentType, title, description, widgetType, languagePath)
	{
		base.Categories = categories;
		base.PlugInAreas = plugInAreas;
		base.SortOrder = sortOrder;
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.DefaultComponentDefinition" /> class.
	///       </summary>
	/// <param name="attributedComponentType">Type of the attributed component.</param>
	/// <param name="title">The title.</param>
	/// <param name="description">The description.</param>
	/// <param name="widgetType">Type of the widget.</param>
	/// <param name="languagePath">The path to the node in the localization resources.</param>
	public DefaultComponentDefinition(Type attributedComponentType, string title, string description, string widgetType, string languagePath)
		: base(widgetType, title, description)
	{
		_attributedComponentType = attributedComponentType;
		base.LanguagePath = languagePath;
	}

	/// <summary>
	///       Creates the component corresponding to this component definition.
	///       </summary>
	/// <returns>
	///       A new instance of an <see cref="T:EPiServer.Shell.ViewComposition.IComponent" />.
	///       </returns>
	/// <remarks>This method does not perform any access control validation.</remarks>
	public override IComponent CreateComponent()
	{
		return CreateComponent(_attributedComponentType);
	}
}
