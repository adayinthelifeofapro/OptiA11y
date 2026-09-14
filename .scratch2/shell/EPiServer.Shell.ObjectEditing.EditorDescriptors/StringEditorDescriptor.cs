namespace EPiServer.Shell.ObjectEditing.EditorDescriptors;

/// <summary>
///       Editor descriptor that will create a standard input to edit strings.
///       </summary>
[EditorDescriptorRegistration(TargetType = typeof(string))]
public class StringEditorDescriptor : EditorDescriptor
{
	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ObjectEditing.EditorDescriptors.StringEditorDescriptor" /> class.
	///       </summary>
	public StringEditorDescriptor()
	{
		base.ClientEditingClass = "dijit/form/ValidationTextBox";
	}
}
