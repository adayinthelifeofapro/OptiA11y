namespace EPiServer.Shell.ObjectEditing.EditorDescriptors;

/// <summary>
///       Editor descriptor that will create a standard input with validation to be able to edit short integer type.
///       </summary>
[EditorDescriptorRegistration(TargetType = typeof(short))]
[EditorDescriptorRegistration(TargetType = typeof(short?))]
internal class ShortEditorDescriptor : NumericEditorDescriptor
{
	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ObjectEditing.EditorDescriptors.ShortEditorDescriptor" /> class.
	///       </summary>
	public ShortEditorDescriptor()
		: base(short.MinValue, short.MaxValue)
	{
	}
}
