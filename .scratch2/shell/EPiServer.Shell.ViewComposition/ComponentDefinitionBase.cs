using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using EPiServer.Framework.Localization;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Helper base class for component definitions
///       </summary>
[DebuggerDisplay("Title={Title}, DefinitionName={DefinitionName}, Category={Category}")]
public abstract class ComponentDefinitionBase : PluggableComponentDefinitionBase, IComponentDefinition, IPluggableComponentDefinition, IContainerMatcher, IEquatable<IComponentDefinition>
{
	private ISettingsDictionary _settings;

	private string _description;

	private string _title;

	[CompilerGenerated]
	private LocalizationService _003CLocalizationService_003Ek__BackingField;

	/// <summary>
	///       Gets or sets the initial settings for components of this type.
	///       </summary>
	/// <value>The initial settings for components.</value>
	public virtual ISettingsDictionary Settings => _settings ?? (_settings = new SettingsDictionary());

	/// <summary>
	///       The service used for localization.
	///       </summary>
	/// <remarks>If this is not set the static instance <see cref="P:EPiServer.Framework.Localization.LocalizationService.Current" /> will be used.</remarks>
	protected LocalizationService LocalizationService => _003CLocalizationService_003Ek__BackingField ?? LocalizationService.Current;

	/// <summary>
	///       Unique name of the definition.
	///       </summary>
	/// <value>The unique name that is used to create new <see cref="T:EPiServer.Shell.ViewComposition.IComponent" />s from the <see cref="T:EPiServer.Shell.ViewComposition.IComponentProvider" />s.</value>
	public virtual string DefinitionName => GetType().FullName;

	/// <summary>
	///       Name of the widget type to use for displaying the component.
	///       </summary>
	/// <value>The name of the widget type to use for displaying the component.</value>
	public string WidgetType { get; protected set; }

	/// <summary>
	///       The title for the component. 
	///       </summary>
	/// <returns>A string with the title.</returns>
	/// <remarks>
	///       If a <see cref="P:EPiServer.Shell.ViewComposition.ComponentDefinitionBase.LanguagePath" /> has been defined, the return value for DisplayName will be the translated 
	///       text from <see cref="P:EPiServer.Shell.ViewComposition.ComponentDefinitionBase.LanguagePath" /> + "/title".
	///       </remarks>
	public virtual string Title
	{
		get
		{
			if (LanguagePath != null)
			{
				string text = LocalizationService.GetString(LanguagePath + "/title");
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
				return _title;
			}
			return _title;
		}
		set
		{
			_title = value;
		}
	}

	/// <summary>
	///       Description of the component.
	///       </summary>
	/// <value>The description.</value>
	/// <remarks>
	///       If the <see cref="P:EPiServer.Shell.ViewComposition.ComponentDefinitionBase.LanguagePath" /> property has been defined, the return value for Description will be the 
	///       translated text from <see cref="P:EPiServer.Shell.ViewComposition.ComponentDefinitionBase.LanguagePath" /> + "/description". 
	///       </remarks>
	public virtual string Description
	{
		get
		{
			if (LanguagePath != null)
			{
				string text = LocalizationService.GetString(LanguagePath + "/description");
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
				return _description;
			}
			return _description;
		}
		set
		{
			_description = value;
		}
	}

	/// <summary>
	///       Path to node in language files where translation can be found.
	///       </summary>
	/// <remarks>
	///       Set this property to the path of the XML element that contains the 
	///       displayname and description elements in one of your localization providers.
	///       (for instance an xml file in the /lang directory.) 
	///       </remarks>
	public string LanguagePath { get; set; }

	/// <summary>
	///       Gets or sets the category for this component. Default implementation returns an empty string;
	///       </summary>
	/// <value>The category.</value>
	public IEnumerable<string> Categories { get; protected set; }

	/// <summary>
	///       Gets the sort order.
	///       </summary>
	public int SortOrder { get; protected set; }

	/// <summary>
	///       Initializes a new instance of the ComponentDefinitionBase class.
	///       </summary>
	/// <param name="widgetType">The type of the widget used for displaying the component.</param>
	protected ComponentDefinitionBase(string widgetType)
		: this(widgetType, null)
	{
	}

	/// <summary>
	///       Initializes a new instance of the ComponentDefinitionBase class using a <see cref="T:EPiServer.Framework.Localization.LocalizationService" />.
	///       </summary>
	/// <param name="widgetType">The type of the widget used for displaying the component.</param>
	/// <param name="localizationService">The service used for localization.</param>
	protected ComponentDefinitionBase(string widgetType, LocalizationService localizationService)
		: this(widgetType, null, null)
	{
		_003CLocalizationService_003Ek__BackingField = localizationService;
	}

	/// <summary>
	///       Initializes a new instance of the ComponentDefinitionBase class with a title and description
	///       </summary>
	/// <param name="widgetType">The type of the widget used for displaying the component.</param>
	/// <param name="title">The title</param>
	/// <param name="description">The description</param>
	protected ComponentDefinitionBase(string widgetType, string title, string description)
	{
		_title = title;
		_description = description;
		base.IsAvailableForUserSelection = true;
		WidgetType = widgetType;
		SortOrder = 100;
		Categories = Array.Empty<string>();
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
		return CreateComponent(GetType());
	}

	/// <summary>
	///       Creates a <see cref="T:EPiServer.Shell.ViewComposition.DefaultComponent" /> instance.
	///       </summary>
	/// <param name="attributedType">Type of the attributed component.</param>
	/// <returns>A <see cref="T:EPiServer.Shell.ViewComposition.DefaultComponent" /> instance</returns>
	protected IComponent CreateComponent(Type attributedType)
	{
		ISettingsDictionary settings = null;
		if (Settings != null)
		{
			settings = Settings.Copy();
		}
		return new DefaultComponent(this, attributedType, settings)
		{
			SortOrder = SortOrder
		};
	}

	/// <summary>
	///       Compares this instance to another <see cref="T:EPiServer.Shell.ViewComposition.IComponentDefinition" /> instanse.
	///       </summary>
	/// <param name="other">The other instance.</param>
	/// <returns>True if both instances are have the same name</returns>
	public bool Equals(IComponentDefinition other)
	{
		if (other != null)
		{
			return DefinitionName == other.DefinitionName;
		}
		return false;
	}
}
