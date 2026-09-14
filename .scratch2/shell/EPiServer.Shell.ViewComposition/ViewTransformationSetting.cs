namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Contains settings to be able to alter which components should be contained in a view.
///       </summary>
public class ViewTransformationSetting
{
	/// <summary>
	///       Gets or sets the plug in area.
	///       </summary>
	/// <value>The plug in area to match.</value>
	public string PlugInArea { get; set; }

	/// <summary>
	///       Gets or sets the <see cref="P:EPiServer.Shell.ViewComposition.ViewTransformationSetting.TransformationType" /> of the modification.
	///       </summary>
	/// <value>The <see cref="P:EPiServer.Shell.ViewComposition.ViewTransformationSetting.TransformationType" /> of the modification.</value>
	public TransformationType TransformationType { get; set; }

	/// <summary>
	///       Gets or sets the definition name for the component.
	///       </summary>
	/// <value>The definition name for the component.</value>
	public string DefinitionName { get; set; }
}
