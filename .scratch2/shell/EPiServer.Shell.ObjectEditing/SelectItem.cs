namespace EPiServer.Shell.ObjectEditing;

/// <summary>
///       Default implementation of <see cref="T:EPiServer.Shell.ObjectEditing.ISelectItem" /></summary>
public class SelectItem : ISelectItem
{
	/// <summary>
	///       Gets the value.
	///       </summary>
	public object Value { get; set; }

	/// <summary>
	///       Gets the text.
	///       </summary>
	public string Text { get; set; }
}
