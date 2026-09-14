namespace EPiServer.Shell.ObjectEditing.EditorDescriptors;

/// <summary>
///       Editor descriptor that will create a standard input with validation to be able to edit unsigned short integer type.
///       </summary>
[EditorDescriptorRegistration(TargetType = typeof(ushort))]
[EditorDescriptorRegistration(TargetType = typeof(ushort?))]
public class UnsignedShortEditorDescriptor : NumericEditorDescriptor
{
	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ObjectEditing.EditorDescriptors.UnsignedShortEditorDescriptor" /> class.
	///       </summary>
	public UnsignedShortEditorDescriptor()
		: base((ushort)0, ushort.MaxValue)
	{
	}
}
