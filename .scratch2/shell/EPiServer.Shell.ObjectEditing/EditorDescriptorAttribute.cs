using System;

namespace EPiServer.Shell.ObjectEditing;

/// <summary>
///       Provides default editor settings for a class or property. These settings are applied first so specific attributes will
///       override the settings from the editor descriptor.
///       </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
public sealed class EditorDescriptorAttribute : Attribute
{
	/// <summary>
	///       Gets or sets the editor descriptor type that you want to use.
	///       </summary>
	/// <value>The editor descriptor type.</value>
	/// <remarks>The settings for the editor types can be overridden with the specific attributes on <see cref="T:EPiServer.Shell.ObjectEditing.ClientEditorAttribute" />.</remarks>
	public Type EditorDescriptorType { get; set; }
}
