namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Defines a view transformation element.
///       </summary>
public class ViewTransformationDetails
{
	/// <summary>
	///       The type of the transformation.
	///       </summary>
	/// <value>The type of the transformation.</value>
	public TransformationType TransformationType { get; set; }

	/// <summary>
	///       The plug area that is used to match a component.
	///       </summary>
	/// <value>The plug in area.</value>
	public string PlugInArea { get; set; }

	/// <summary>
	///       The definition name of the component to match.
	///       </summary>
	/// <value>The definition name for the component.</value>
	/// <remarks>This should match either the <see cref="P:EPiServer.Shell.ViewComposition.IComponent.DefinitionName" /> or <see cref="P:EPiServer.Shell.ViewComposition.IComponentDefinition.DefinitionName" /> depending on the type of transformation.</remarks>
	public string DefinitionName { get; set; }

	/// <summary>
	///       The name of the view transformation.
	///       </summary>
	public string Name { get; set; }
}
