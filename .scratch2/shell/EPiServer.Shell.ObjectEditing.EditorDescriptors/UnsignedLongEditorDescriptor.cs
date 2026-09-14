namespace EPiServer.Shell.ObjectEditing.EditorDescriptors;

/// <summary>
///       Editor descriptor that will create a standard input with validation to be able to edit unsigned long integer type.
///       </summary>
[EditorDescriptorRegistration(TargetType = typeof(ulong))]
[EditorDescriptorRegistration(TargetType = typeof(ulong?))]
public class UnsignedLongEditorDescriptor : NumericEditorDescriptor
{
	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ObjectEditing.EditorDescriptors.UnsignedLongEditorDescriptor" /> class.
	///       </summary>
	public UnsignedLongEditorDescriptor()
		: base(0uL, ulong.MaxValue)
	{
	}
}
