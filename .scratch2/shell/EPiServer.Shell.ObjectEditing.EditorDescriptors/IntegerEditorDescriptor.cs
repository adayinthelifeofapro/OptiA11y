namespace EPiServer.Shell.ObjectEditing.EditorDescriptors;

/// <summary>
///       Editor descriptor that will create a standard input with validation to be able to edit integer type.
///       </summary>
[EditorDescriptorRegistration(TargetType = typeof(int))]
[EditorDescriptorRegistration(TargetType = typeof(int?))]
public class IntegerEditorDescriptor : NumericEditorDescriptor
{
	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ObjectEditing.EditorDescriptors.IntegerEditorDescriptor" /> class.
	///       </summary>
	public IntegerEditorDescriptor()
		: base(int.MinValue, int.MaxValue)
	{
	}
}
