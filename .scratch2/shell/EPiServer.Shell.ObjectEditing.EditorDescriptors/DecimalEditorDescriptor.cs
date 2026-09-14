namespace EPiServer.Shell.ObjectEditing.EditorDescriptors;

/// <summary>
///       Editor descriptor that will create a standard input with validation to be able to edit decimal type.
///       </summary>
[EditorDescriptorRegistration(TargetType = typeof(decimal))]
[EditorDescriptorRegistration(TargetType = typeof(decimal?))]
public class DecimalEditorDescriptor : FloatingPointNumericEditorDescriptor
{
	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ObjectEditing.EditorDescriptors.DecimalEditorDescriptor" /> class.
	///       </summary>
	public DecimalEditorDescriptor()
		: base(decimal.MinValue, decimal.MaxValue, 29)
	{
	}
}
