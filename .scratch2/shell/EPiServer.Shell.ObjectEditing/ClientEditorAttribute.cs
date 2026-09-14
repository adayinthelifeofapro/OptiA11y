using System;

namespace EPiServer.Shell.ObjectEditing;

/// <summary>
///       Attribute to overwrite system's default editor. Can be applied to either classes or properties
///       </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
public sealed class ClientEditorAttribute : Attribute
{
	/// <summary>
	///       Gets or sets the client editing class, usually a DOJO widget.
	///       </summary>
	/// <value>The client editing class.</value>
	public string ClientEditingClass { get; set; }

	/// <summary>
	///       Gets or sets the client editing class package, which needs to be required, usually a dojo package.
	///       Leave it empty if package name is the same to class name.
	///       </summary>
	/// <value>
	///       The client editing class package.
	///       </value>
	public string ClientEditingPackage { get; set; }

	/// <summary>
	///       Gets or sets the layout class.
	///       </summary>
	/// <value>
	///       The layout class.
	///       </value>
	public string LayoutClass { get; set; }

	/// <summary>
	///       Gets or sets the default binding value.
	///       </summary>
	/// <value>
	///       The default binding value.
	///       </value>
	public string DefaultValue { get; set; }

	/// <summary>
	///       Gets or sets the editors initial configuration. Should be a string in JSON format. This will be passed to the editor class's constructor
	///       </summary>
	/// <value>
	///       The editors configuration.
	///       </value>
	public string EditorConfiguration { get; set; }

	/// <summary>
	///       Gets or sets the configuration of the overlay. Should be a string in JSON format. This will be passed to the editor class's constructor
	///       </summary>
	/// <value>
	///       The overlays configuration.
	///       </value>
	public string OverlayConfiguration { get; set; }

	/// <summary>
	///       Gets or sets the type of the selection factory.
	///       </summary>
	/// <value>
	///       The type of the selection factory.
	///       </value>
	public Type SelectionFactoryType { get; set; }

	/// <summary>
	///       When true, then editor will be loaded as module
	///       </summary>
	public bool IsJavascriptModule { get; set; }
}
