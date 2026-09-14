namespace EPiServer.Shell.ObjectEditing.EditorDescriptors;

/// <summary>
///       Editor descriptor that will create a standard input with validation to be able to edit unsigned integer type.
///       </summary>
[EditorDescriptorRegistration(TargetType = typeof(uint))]
[EditorDescriptorRegistration(TargetType = typeof(uint?))]
public class UnsignedIntegerEditorDescriptor : NumericEditorDescriptor
{
	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ObjectEditing.EditorDescriptors.UnsignedIntegerEditorDescriptor" /> class.
	///       </summary>
	public UnsignedIntegerEditorDescriptor()
		: base(0u, uint.MaxValue)
	{
	}
}
