using System;
using EPiServer.ServiceLocation;

namespace EPiServer.Shell.ObjectEditing.EditorDescriptors;

/// <summary>
///       Provides default editor settings for a class or property. These settings are applied first so specific attributes will
///       override the settings from the editor descriptor.
///       </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class EditorDescriptorRegistrationAttribute : ServicePlugInAttributeBase
{
	/// <summary>
	///       Gets or sets the editor descriptor type that you want to use.
	///       </summary>
	/// <value>The editor descriptor type.</value>
	/// <remarks>The settings for the editor types can be overridden with the specific attributes on <see cref="T:EPiServer.Shell.ObjectEditing.ClientEditorAttribute" />.</remarks>
	public Type TargetType { get; set; }

	/// <summary>
	///       Gets or sets the ui hint. It provides possibility to register different editors to a type with diffirent hints.
	///       </summary>
	/// <value>
	///       The ui hint.
	///       </value>
	public string UIHint { get; set; }

	/// <summary>
	///       Gets or sets a value indicating whether the descriptors should be applied before this descriptor.
	///       </summary>
	/// <value>
	///   <c>true</c> if the base descriptors should be applied before this descriptor; otherwise, <c>false</c>.
	///       </value>
	/// <remarks>This can be used if you want to have an editor descritor that only wants to change some of the default behaviour.
	///       Register an editor descriptor for a specific UI Hint and set this property to true and the default descriptors will be
	///       called before your descriptor.</remarks>
	public EditorDescriptorBehavior EditorDescriptorBehavior { get; set; }

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ObjectEditing.EditorDescriptors.EditorDescriptorRegistrationAttribute" /> class.
	///       </summary>
	public EditorDescriptorRegistrationAttribute()
		: base(typeof(EditorDescriptor))
	{
		EditorDescriptorBehavior = EditorDescriptorBehavior.Default;
	}
}
