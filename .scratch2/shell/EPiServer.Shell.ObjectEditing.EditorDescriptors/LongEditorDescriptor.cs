namespace EPiServer.Shell.ObjectEditing.EditorDescriptors;

/// <summary>
///       Editor descriptor that will create a standard input with validation to be able to edit long integer type.
///       </summary>
[EditorDescriptorRegistration(TargetType = typeof(long))]
[EditorDescriptorRegistration(TargetType = typeof(long?))]
internal class LongEditorDescriptor : NumericEditorDescriptor
{
	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ObjectEditing.EditorDescriptors.LongEditorDescriptor" /> class.
	///       </summary>
	public LongEditorDescriptor()
		: base(long.MinValue, long.MaxValue)
	{
	}
}
