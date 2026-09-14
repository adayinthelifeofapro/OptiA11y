using System;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Default <see cref="T:EPiServer.Shell.ViewComposition.IComponent" /> used for components not having any special requirements for client side state transfer.
///       </summary>
internal class DefaultComponent : ComponentBase
{
	private readonly IComponentDefinition _componentDefinition;

	private readonly Type _attributedComponentType;

	/// <summary>
	///       Unique name of the component definition.
	///       </summary>
	public override string DefinitionName => _componentDefinition.DefinitionName;

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.DefaultComponent" /> class.
	///       </summary>
	/// <param name="definition">The definition of the component.</param>
	/// <param name="attributedComponentType">The attributed type, used for resolving owner module name.</param>
	public DefaultComponent(IComponentDefinition definition, Type attributedComponentType)
		: this(definition, attributedComponentType, null)
	{
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.DefaultComponent" /> class.
	///       </summary>
	/// <param name="definition">The definition of the component.</param>
	/// <param name="attributedComponentType">The attributed type, used for resolving owner module name.</param>
	/// <param name="settings">The initial settings.</param>
	public DefaultComponent(IComponentDefinition definition, Type attributedComponentType, ISettingsDictionary settings)
		: base(definition.WidgetType, settings)
	{
		ArgumentNullException.ThrowIfNull(definition, "definition");
		_componentDefinition = definition;
		_attributedComponentType = attributedComponentType;
		if (!string.IsNullOrEmpty(_componentDefinition.Title))
		{
			base.Settings.Add(new Setting("heading", _componentDefinition.Title, personalizable: false));
		}
	}

	/// <inheritdoc />
	protected override string GetModuleName()
	{
		return GetModuleName(_attributedComponentType);
	}
}
