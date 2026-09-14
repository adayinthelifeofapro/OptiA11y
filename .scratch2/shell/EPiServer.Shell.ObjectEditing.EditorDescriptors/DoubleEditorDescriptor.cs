namespace EPiServer.Shell.ObjectEditing.EditorDescriptors;

/// <summary>
///       Editor descriptor that will create a standard input with validation to be able to edit double type.
///       </summary>
[EditorDescriptorRegistration(TargetType = typeof(double))]
[EditorDescriptorRegistration(TargetType = typeof(double?))]
public class DoubleEditorDescriptor : FloatingPointNumericEditorDescriptor
{
	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ObjectEditing.EditorDescriptors.DoubleEditorDescriptor" /> class.
	///       </summary>
	public DoubleEditorDescriptor()
		: base(double.MinValue, double.MaxValue, 16)
	{
	}
}
