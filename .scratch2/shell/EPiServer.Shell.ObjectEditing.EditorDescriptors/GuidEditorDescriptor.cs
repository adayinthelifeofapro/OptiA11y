using System;

namespace EPiServer.Shell.ObjectEditing.EditorDescriptors;

/// <summary>
///       Editor descriptor that will create a hidden input to edit a Guid value.
///       </summary>
[EditorDescriptorRegistration(TargetType = typeof(Guid))]
[EditorDescriptorRegistration(TargetType = typeof(Guid?))]
public class GuidEditorDescriptor : EditorDescriptor
{
	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ObjectEditing.EditorDescriptors.GuidEditorDescriptor" /> class.
	///       </summary>
	public GuidEditorDescriptor()
	{
		base.ClientEditingClass = "dijit/form/ValidationTextBox";
	}
}
