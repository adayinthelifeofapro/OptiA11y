namespace EPiServer.Shell.ObjectEditing.EditorDescriptors;

/// <summary>
///       Base class for floating point numeric types' editor descriptor
///       </summary>
public abstract class FloatingPointNumericEditorDescriptor : NumericEditorDescriptor
{
	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ObjectEditing.EditorDescriptors.FloatingPointNumericEditorDescriptor" /> class.
	///       </summary>
	protected FloatingPointNumericEditorDescriptor(object min, object max, int significantDigits)
		: base(min, max, significantDigits)
	{
		base.ClientEditingClass = "dijit/form/NumberTextBox";
	}
}
