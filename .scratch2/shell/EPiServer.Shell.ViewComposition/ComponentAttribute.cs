using System;
using System.Collections.Generic;
using System.Linq;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Defines a user interface component.
///       </summary>
/// <remarks>
///       This attribute will create a new instance of <see cref="T:EPiServer.Shell.ViewComposition.IComponentDefinition" /> with the settings provided in the attribute properties.
///       </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ComponentAttribute : Attribute
{
	/// <summary>
	///       Uses the <see cref="T:EPiServer.Shell.ViewComposition.IComponent" /> type as base for the <see cref="T:EPiServer.Shell.ViewComposition.IComponentDefinition" />.
	///       </summary>
	private class SelfTypedComponentDefinition : DefaultComponentDefinition
	{
		private readonly Type _componentType;

		public override string DefinitionName => CreateComponent().DefinitionName ?? base.DefinitionName;

		public SelfTypedComponentDefinition(Type componentType, ComponentAttribute attribute)
			: base(componentType, attribute.Title ?? componentType.Name, attribute.Description, attribute.Categories?.Split(',', StringSplitOptions.RemoveEmptyEntries), attribute.WidgetType, attribute.LanguagePath, attribute.PlugInAreas?.Split(',', StringSplitOptions.RemoveEmptyEntries), attribute.SortOrder)
		{
			ArgumentNullException.ThrowIfNull(componentType, "componentType");
			ArgumentNullException.ThrowIfNull(attribute, "attribute");
			if (componentType.GetConstructor(Type.EmptyTypes) == null)
			{
				throw new ArgumentException("The component type " + componentType?.ToString() + " does not have a parameterless constructor. This is required for components with the [Component] attribute.", "componentType");
			}
			base.IsAvailableForUserSelection = attribute.IsAvailableForUserSelection;
			_componentType = componentType;
		}

		public override IComponent CreateComponent()
		{
			return Activator.CreateInstance(_componentType) as IComponent;
		}
	}

	private readonly ISettingsDictionary _settings = new SettingsDictionary();

	/// <summary>
	///       Gets or sets the list of roles that are allowed to use this component.
	///       </summary>
	/// <value>The role list .</value>
	public string AllowedRoles { get; set; }

	/// <summary>
	///       Name of the widget type to use for displaying the component.
	///       </summary>
	/// <value>The name of the widget type to use for displaying the component.</value>
	public virtual string WidgetType { get; set; }

	/// <summary>
	///       Title used in the UI both for listings and as default for component instances. Should be localized.
	///       </summary>
	/// <value>The title.</value>
	public virtual string Title { get; set; }

	/// <summary>
	///       Description of the component. Should be localized.
	///       </summary>
	/// <value>The description.</value>
	public virtual string Description { get; set; }

	/// <summary>
	///       A comma separated string of categories for the component.
	///       </summary>
	public virtual string Categories { get; set; }

	/// <summary>
	///       Gets or sets the sort order for this component.
	///       </summary>
	public virtual int SortOrder { get; set; }

	/// <summary>
	///       Gets or sets a value indicating whether this component can be added in the ui by a user.
	///       </summary>
	public bool IsAvailableForUserSelection { get; set; }

	/// <summary>
	///       Gets or sets the plug in areas that the component should automatically plug into.
	///       </summary>
	/// <value>The plug in areas.</value>
	/// <remarks>The input should be in the format of a comma separated string if you want to
	///       plug into several areas.</remarks>
	public virtual string PlugInAreas { get; set; }

	/// <summary>
	///       Path to node in language files where translation can be found.
	///       </summary>
	/// <remarks>
	///       Set this property to the path of the XML element that contains the 
	///       displayname and description elements in one of your localization providers.
	///       (for instance an xml file in the /lang directory.) 
	///       </remarks>
	public virtual string LanguagePath { get; set; }

	/// <summary>
	///       Gets or sets the initial settings for components of this type.
	///       </summary>
	/// <value>The initial settings for components.</value>
	protected virtual ISettingsDictionary Settings => _settings;

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.ComponentAttribute" /> class.
	///       </summary>
	public ComponentAttribute()
	{
		IsAvailableForUserSelection = true;
	}

	/// <summary>
	///       Creates the component definition.
	///       </summary>
	/// <param name="attributedType">The <see cref="T:System.Type" /> of the component.</param>
	/// <returns>A new instance of a <see cref="T:EPiServer.Shell.ViewComposition.DefaultComponentDefinition" />.</returns>
	public virtual IComponentDefinition CreateComponentDefinition(Type attributedType)
	{
		IComponentDefinition componentDefinition = null;
		if (typeof(IComponentDefinition).IsAssignableFrom(attributedType))
		{
			componentDefinition = Activator.CreateInstance(attributedType) as IComponentDefinition;
		}
		else if (typeof(IComponent).IsAssignableFrom(attributedType))
		{
			componentDefinition = new SelfTypedComponentDefinition(attributedType, this);
		}
		else
		{
			string[] plugInAreas = PlugInAreas?.Split(',', StringSplitOptions.RemoveEmptyEntries);
			string[] categories = Categories?.Split(',', StringSplitOptions.RemoveEmptyEntries);
			DefaultComponentDefinition defaultComponentDefinition = new DefaultComponentDefinition(attributedType, Title, Description, categories, WidgetType, LanguagePath, plugInAreas, SortOrder);
			foreach (KeyValuePair<string, object> setting in Settings)
			{
				defaultComponentDefinition.Settings.Add(setting);
			}
			componentDefinition = defaultComponentDefinition;
		}
		ApplyAllowedRoles(componentDefinition);
		return componentDefinition;
	}

	private void ApplyAllowedRoles(IComponentDefinition componentDefinition)
	{
		if (componentDefinition != null && !string.IsNullOrEmpty(AllowedRoles))
		{
			string[] array = (from s in AllowedRoles.Split(',', StringSplitOptions.RemoveEmptyEntries)
				select s.Trim()).ToArray();
			foreach (string item in array)
			{
				componentDefinition.AllowedRoles.Add(item);
			}
		}
	}
}
