namespace EPiServer.Shell.ObjectEditing.EditorDescriptors;

/// <summary>
///       Editor descriptor that will create a standard input with validation to be able to edit float type.
///       </summary>
[EditorDescriptorRegistration(TargetType = typeof(float))]
[EditorDescriptorRegistration(TargetType = typeof(float?))]
public class FloatEditorDescriptor : FloatingPointNumericEditorDescriptor
{
	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ObjectEditing.EditorDescriptors.FloatEditorDescriptor" /> class.
	///       </summary>
	public FloatEditorDescriptor()
		: base(float.MinValue, float.MaxValue, 7)
	{
	}
}
